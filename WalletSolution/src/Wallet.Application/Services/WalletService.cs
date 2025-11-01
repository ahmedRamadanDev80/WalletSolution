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
        private readonly IConfigurationRuleRepository _ruleRepo;

        public WalletService(IWalletRepository walletRepo, IWalletTransactionRepository txRepo, IUnitOfWork uow, IConfigurationRuleRepository ruleRepo)
        {
            _walletRepo = walletRepo;
            _txRepo = txRepo;
            _uow = uow;
            _ruleRepo = ruleRepo;
        }

        // New signature: accepts userId (controller will extract from JWT) and serviceId optional
        public async Task<WalletDto> EarnAsync(Guid userId, decimal amountMoney, Guid? serviceId, string? externalRef, string? desc, CancellationToken ct)
        {
            if (amountMoney <= 0) throw new ArgumentException(nameof(amountMoney));

            var wallet = await _walletRepo.GetByUserIdAsync(userId, ct);
            if (wallet == null) throw new InvalidOperationException("Wallet not found");

            // idempotency check
            if (!string.IsNullOrEmpty(externalRef) && await _txRepo.ExistsByExternalReferenceAsync(wallet.Id, externalRef, ct))
            {
                // Already applied — return current state
                return new WalletDto(wallet.UserId, wallet.Balance);
            }

            // Determine points to add using config rule if available
            long pointsToAdd;
            if (serviceId.HasValue)
            {
                var rule = await _ruleRepo.GetByServiceAndTypeAsync(serviceId.Value, "EARNING", ct);
                if (rule != null)
                {
                    // formula: floor( amountMoney / BaseAmount * PointsPerBaseAmount )
                    var factor = Math.Floor(amountMoney / rule.BaseAmount);
                    pointsToAdd = (long)(factor * rule.PointsPerBaseAmount);
                }
                else
                {
                    var defaultRule = await _ruleRepo.GetDefaultRuleAsync("EARNING", ct);
                    if (defaultRule != null)
                    {
                        var factor = Math.Floor(amountMoney / defaultRule.BaseAmount);
                        pointsToAdd = (long)(factor * defaultRule.PointsPerBaseAmount);
                    }
                    else
                    {
                        // fallback simple rule
                        pointsToAdd = (long)Math.Floor(amountMoney);
                    }
                }
            }
            else
            {
                var defaultRule = await _ruleRepo.GetDefaultRuleAsync("EARNING", ct);
                if (defaultRule != null)
                {
                    var factor = Math.Floor(amountMoney / defaultRule.BaseAmount);
                    pointsToAdd = (long)(factor * defaultRule.PointsPerBaseAmount);
                }
                else
                {
                    pointsToAdd = (long)Math.Floor(amountMoney);
                }
            }

            if (pointsToAdd <= 0)
            {
                // nothing to add (e.g., amount less than base amount)
                return new WalletDto(wallet.UserId, wallet.Balance);
            }

            int attempts = 0;
            while (true)
            {
                attempts++;
                try
                {
                    await _uow.BeginTransactionAsync(ct);

                    wallet.AddPoints(pointsToAdd); // ensure Wallet entity has AddPoints(long)
                    _walletRepo.Update(wallet);

                    var tx = new WalletTransaction(
                        wallet.Id,
                        "EARN",
                        pointsToAdd,
                        wallet.Balance,
                        externalRef,
                        desc
                    );

                    await _txRepo.AddAsync(tx, ct);

                    await _uow.SaveChangesAsync(ct);
                    await _uow.CommitAsync(ct);

                    return new WalletDto(wallet.UserId, wallet.Balance);
                }
                catch (DbUpdateConcurrencyException)
                {
                    await _uow.RollbackAsync(ct);
                    if (attempts >= 3) throw;
                    // reload wallet and retry
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

        public async Task<WalletDto> BurnAsync(Guid userId, decimal amountMoneyOrPoints, string? externalRef, string? desc, CancellationToken ct)
        {
            if (amountMoneyOrPoints <= 0) throw new ArgumentException(nameof(amountMoneyOrPoints));

            var wallet = await _walletRepo.GetByUserIdAsync(userId, ct) ?? throw new InvalidOperationException("Wallet not found");

            if (!string.IsNullOrEmpty(externalRef) && await _txRepo.ExistsByExternalReferenceAsync(wallet.Id, externalRef, ct))
            {
                return new WalletDto(wallet.UserId, wallet.Balance);
            }

            // For burning we assume 1 point = 1 SAR unless you have burn-specific rules (could extend)
            long pointsToBurn = (long)Math.Floor(amountMoneyOrPoints);

            int attempts = 0;
            while (true)
            {
                attempts++;
                try
                {
                    await _uow.BeginTransactionAsync(ct);

                    if (wallet.Balance < pointsToBurn) throw new InvalidOperationException("Insufficient points");

                    wallet.BurnPoints(pointsToBurn);
                    _walletRepo.Update(wallet);

                    var tx = new WalletTransaction(wallet.Id, "BURN", pointsToBurn, wallet.Balance, externalRef, desc);
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
