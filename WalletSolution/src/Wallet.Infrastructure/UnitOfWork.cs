using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Application.Interfaces;
using Wallet.Infrastructure.Data;

namespace Wallet.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WalletDbContext _db;
        private IDbContextTransaction? _currentTransaction;

        public UnitOfWork(WalletDbContext db) { _db = db; }

        public async Task BeginTransactionAsync(CancellationToken ct = default)
        {
            if (_currentTransaction != null) return;
            _currentTransaction = await _db.Database.BeginTransactionAsync(ct);
        }

        public async Task CommitAsync(CancellationToken ct = default)
        {
            if (_currentTransaction == null) return;
            await _db.SaveChangesAsync(ct);
            await _currentTransaction.CommitAsync(ct);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        public async Task RollbackAsync(CancellationToken ct = default)
        {
            if (_currentTransaction == null) return;
            await _currentTransaction.RollbackAsync(ct);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _db.SaveChangesAsync(ct);
        }
    }
}
