using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Application.DTOs;
using Wallet.Application.Interfaces;
using Wallet.Core.Entities;

namespace Wallet.Application.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepo;
        private readonly IWalletTransactionRepository _txRepo;
        private readonly IUnitOfWork _uow;

        public WalletService(IWalletRepository walletRepo, IWalletTransactionRepository txRepo, IUnitOfWork uow)
        {
            _walletRepo = walletRepo;
            _txRepo = txRepo;
            _uow = uow;
        }

        public async Task<WalletDto> EarnAsync(Guid userId, long amount, string? externalRef, string? desc, CancellationToken ct)
        {
            if (amount <= 0) throw new ArgumentException(nameof(amount));

            var wallet = await _walletRepo.GetByUserIdAsync(userId, ct) ?? throw new InvalidOperationException("Wallet not found");
            // idempotency check
            if (!string.IsNullOrEmpty(externalRef) && await _txRepo.ExistsByExternalReferenceAsync(wallet.Id, externalRef, ct))
            {
                // return current state (idempotent)
                return new WalletDto(wallet.UserId, wallet.Balance);
            }

            // transaction + optimistic concurrency with retries
            int attempts = 0;
            while (true)
            {
                attempts++;
                try
                {
                    await _uow.BeginTransactionAsync(ct);

                    wallet.AddPoints(amount);
                    _walletRepo.Update(wallet);

                    var tx = new WalletTransaction(wallet.Id, "EARN", amount, wallet.Balance, externalRef, desc);
                    await _txRepo.AddAsync(tx, ct);

                    await _uow.SaveChangesAsync(ct);
                    await _uow.CommitAsync(ct);

                    return new WalletDto(wallet.UserId, wallet.Balance);
                }
                catch (DbUpdateConcurrencyException)
                {
                    await _uow.RollbackAsync(ct);
                    if (attempts >= 3) throw;
                    // refresh wallet from DB and retry
                    wallet = await _walletRepo.GetByUserIdAsync(userId, ct) ?? throw new InvalidOperationException("Wallet missing during retry");
                    continue;
                }
                catch
                {
                    await _uow.RollbackAsync(ct);
                    throw;
                }
            }
        }

        public async Task<WalletDto> BurnAsync(Guid userId, long amount, string? externalRef, string? desc, CancellationToken ct)
        {
            if (amount <= 0) throw new ArgumentException(nameof(amount));
            var wallet = await _walletRepo.GetByUserIdAsync(userId, ct) ?? throw new InvalidOperationException("Wallet not found");

            if (!string.IsNullOrEmpty(externalRef) && await _txRepo.ExistsByExternalReferenceAsync(wallet.Id, externalRef, ct))
            {
                return new WalletDto(wallet.UserId, wallet.Balance);
            }

            int attempts = 0;
            while (true)
            {
                attempts++;
                try
                {
                    await _uow.BeginTransactionAsync(ct);

                    if (wallet.Balance < amount) throw new InvalidOperationException("Insufficient points");

                    wallet.BurnPoints(amount);
                    _walletRepo.Update(wallet);

                    var tx = new WalletTransaction(wallet.Id, "BURN", amount, wallet.Balance, externalRef, desc);
                    await _txRepo.AddAsync(tx, ct);

                    await _uow.SaveChangesAsync(ct);
                    await _uow.CommitAsync(ct);

                    return new WalletDto(wallet.UserId, wallet.Balance);
                }
                catch (DbUpdateConcurrencyException)
                {
                    await _uow.RollbackAsync(ct);
                    if (attempts >= 3) throw;
                    wallet = await _walletRepo.GetByUserIdAsync(userId, ct) ?? throw new InvalidOperationException("Wallet missing during retry");
                    continue;
                }
                catch
                {
                    await _uow.RollbackAsync(ct);
                    throw;
                }
            }
        }

        public async Task<WalletDto?> GetBalanceByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            var wallet = await _walletRepo.GetByUserIdAsync(userId, ct);
            if (wallet == null) return null;
            return new WalletDto(wallet.UserId, wallet.Balance);
        }

        public async Task<PagedResult<WalletTransactionDto>> GetTransactionsAsync(Guid userId, int skip, int take, CancellationToken ct = default)
        {
            // validate paging
            if (skip < 0) skip = 0;
            if (take <= 0) take = 20; // default

            var wallet = await _walletRepo.GetByUserIdAsync(userId, ct);
            if (wallet == null)
                return new PagedResult<WalletTransactionDto>(Array.Empty<WalletTransactionDto>(), 0);

            var txs = await _txRepo.GetTransactionsAsync(wallet.Id, skip, take, ct);
            var total = await _txRepo.CountAsync(wallet.Id, ct);

            var dtos = txs.Select(t => new WalletTransactionDto(
                t.Id,
                t.Type,
                t.Amount,
                t.BalanceAfter,
                t.Description,
                t.ExternalReference,
                t.CreatedAt
            )).ToList().AsReadOnly();

            return new PagedResult<WalletTransactionDto>(dtos, total);
        }

    }
}
