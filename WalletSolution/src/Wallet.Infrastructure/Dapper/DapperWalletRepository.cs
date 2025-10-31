using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Application.Interfaces;

namespace Wallet.Infrastructure.Dapper
{
    public class DapperWalletRepository : IDapperWalletRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<DapperWalletRepository> _logger;
        private readonly int _commandTimeoutSeconds;

        public DapperWalletRepository(IDbConnectionFactory connectionFactory, ILogger<DapperWalletRepository> logger, IOptions<DapperOptions> options)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _commandTimeoutSeconds = options?.Value?.CommandTimeoutSeconds ?? 30;
        }

        public async Task<long> MutateWithPessimisticLockAsync(Guid userId, long amount, string type, string? externalRef, string? description, CancellationToken ct)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be > 0", nameof(amount));
            if (type != "EARN" && type != "BURN") throw new ArgumentException("Invalid type", nameof(type));

            using var conn = _connectionFactory.CreateConnection();
            // open connection (Microsoft.Data.SqlClient supports OpenAsync with cancellation token)
            await ((System.Data.Common.DbConnection)conn).OpenAsync(ct);

            using var tx = conn.BeginTransaction(IsolationLevel.ReadCommitted); // keep short

            try
            {
                // 1) Select wallet row with UPDLOCK to serialize updates on this row
                const string selectSql = @"
                                        SELECT Id, UserId, Balance, RowVersion
                                        FROM Wallets WITH (UPDLOCK, ROWLOCK)
                                        WHERE UserId = @UserId;
                                        ";
                var wallet = await conn.QuerySingleOrDefaultAsync<WalletRow>(
                    new CommandDefinition(selectSql, new { UserId = userId }, transaction: tx, commandTimeout: _commandTimeoutSeconds, cancellationToken: ct)
                );

                if (wallet == null)
                {
                    // create wallet row if missing (adjust if you don't want automatic creation)
                    var newWalletId = Guid.NewGuid();
                    const string insertWalletSql = @"
                                                INSERT INTO Wallets (Id, UserId, Balance)
                                                VALUES (@Id, @UserId, @Balance);
                                                ";
                    await conn.ExecuteAsync(new CommandDefinition(insertWalletSql,
                        new { Id = newWalletId, UserId = userId, Balance = 0L }, transaction: tx, commandTimeout: _commandTimeoutSeconds, cancellationToken: ct));

                    wallet = new WalletRow { Id = newWalletId, UserId = userId, Balance = 0L };
                }

                // 2) Idempotency check (inside tx)
                if (!string.IsNullOrEmpty(externalRef))
                {
                    const string existsSql = @"
                                            SELECT COUNT(1)
                                            FROM WalletTransactions WITH (NOLOCK)
                                            WHERE WalletId = @WalletId AND ExternalReference = @ExternalReference;
                                            ";
                    var exists = await conn.ExecuteScalarAsync<int>(
                        new CommandDefinition(existsSql, new { WalletId = wallet.Id, ExternalReference = externalRef }, transaction: tx, commandTimeout: _commandTimeoutSeconds, cancellationToken: ct)
                    );

                    if (exists > 0)
                    {
                        tx.Commit();
                        return wallet.Balance;
                    }
                }

                // 3) Compute new balance
                long newBalance = wallet.Balance;
                if (type == "EARN")
                {
                    newBalance += amount;
                }
                else // BURN
                {
                    if (wallet.Balance < amount)
                    {
                        tx.Rollback();
                        throw new InvalidOperationException("Insufficient points");
                    }
                    newBalance -= amount;
                }

                // 4) Update wallet
                const string updateSql = @"
                                        UPDATE Wallets
                                        SET Balance = @Balance
                                        WHERE Id = @Id;
                                        ";
                await conn.ExecuteAsync(new CommandDefinition(updateSql, new { Balance = newBalance, Id = wallet.Id }, transaction: tx, commandTimeout: _commandTimeoutSeconds, cancellationToken: ct));

                // 5) Insert wallet transaction
                const string insertTxSql = @"
                                        INSERT INTO WalletTransactions (Id, WalletId, Type, Amount, BalanceAfter, Description, ExternalReference, CreatedAt)
                                        VALUES (@Id, @WalletId, @Type, @Amount, @BalanceAfter, @Description, @ExternalReference, SYSUTCDATETIME());
                                        ";
                await conn.ExecuteAsync(new CommandDefinition(insertTxSql,
                    new
                    {
                        Id = Guid.NewGuid(),
                        WalletId = wallet.Id,
                        Type = type,
                        Amount = amount,
                        BalanceAfter = newBalance,
                        Description = description,
                        ExternalReference = externalRef
                    }, transaction: tx, commandTimeout: _commandTimeoutSeconds, cancellationToken: ct));

                tx.Commit();
                return newBalance;
            }
            catch
            {
                _logger.LogError("Dapper mutate failed for user {UserId}", userId);
                try { tx.Rollback(); } catch { /* ignore */ }
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        private class WalletRow
        {
            public Guid Id { get; set; }
            public Guid UserId { get; set; }
            public long Balance { get; set; }
            public byte[]? RowVersion { get; set; }
        }
    }
}
