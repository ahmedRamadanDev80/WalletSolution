using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Wallet.Application.Interfaces;
using Wallet.Core.Entities;
using Wallet.Infrastructure.Data;
namespace Wallet.Infrastructure.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly WalletDbContext _db;

        public WalletRepository(WalletDbContext db) { _db = db; }

        public async Task CreateAsync(WalletEntity wallet, CancellationToken ct = default)
        {
            await _db.Wallets.AddAsync(wallet, ct);
        }

        public async Task<WalletEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _db.Wallets.FirstOrDefaultAsync(w => w.Id == id, ct);
        }

        public async Task<WalletEntity?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _db.Wallets.FirstOrDefaultAsync(w => w.UserId == userId, ct);
        }

        public void Update(WalletEntity wallet)
        {
            _db.Wallets.Update(wallet);
        }
    }
}
