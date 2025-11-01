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
    public class UserRepository : IUserRepository
    {
        private readonly WalletDbContext _ctx;
        public UserRepository(WalletDbContext ctx) => _ctx = ctx;

        public async Task AddAsync(User user, CancellationToken ct = default) => await _ctx.Users.AddAsync(user, ct);

        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default) => await _ctx.Users.ToListAsync(ct);

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
            await _ctx.Users.FirstOrDefaultAsync(u => u.email == email, ct);

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) => await _ctx.Users.FindAsync(new object[] { id }, ct);

        public void Update(User user) => _ctx.Users.Update(user);

        public async Task SaveChangesAsync(CancellationToken ct = default) => await _ctx.SaveChangesAsync(ct);
    }
}
