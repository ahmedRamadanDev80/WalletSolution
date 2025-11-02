using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Application.Interfaces;
using Wallet.Core.Entities;
using Wallet.Infrastructure.Data;

namespace Wallet.Infrastructure.Repositories
{
    public class WalletTransactionRepository : IWalletTransactionRepository
    {
        private readonly WalletDbContext _db;

        public WalletTransactionRepository(WalletDbContext db) { _db = db; }

        public async Task AddAsync(WalletTransaction tx, CancellationToken ct = default)
        {
            await _db.WalletTransactions.AddAsync(tx, ct);
        }

        public async Task<bool> ExistsByExternalReferenceAsync(Guid walletId, string externalRef, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(externalRef)) return false;
            return await _db.WalletTransactions.AnyAsync(t => t.WalletId == walletId && t.ExternalReference == externalRef, ct);
        }

        public async Task<IReadOnlyList<WalletTransaction>> GetTransactionsAsync(Guid walletId, int skip, int take, CancellationToken ct = default)
        {
            var q = _db.WalletTransactions
                       .Where(t => t.WalletId == walletId)
                       .OrderByDescending(t => t.CreatedAt)
                       .Skip(skip)
                       .Take(take);

            return await q.ToListAsync(ct);
        }

        public async Task<int> CountAsync(Guid walletId, CancellationToken ct = default)
        {
            return await _db.WalletTransactions.CountAsync(t => t.WalletId == walletId, ct);
        }

        public async Task<long> SumAmountByTypeAsync(string type, CancellationToken ct = default)
        {
            return await _db.WalletTransactions
                .Where(t => t.Type == type)
                .SumAsync(t => (long?)t.Amount, ct) ?? 0L;
        }

    }
}
