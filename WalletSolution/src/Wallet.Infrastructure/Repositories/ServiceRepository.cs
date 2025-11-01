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
    public class ServiceRepository : IServiceRepository
    {
        private readonly WalletDbContext _ctx;
        public ServiceRepository(WalletDbContext ctx) => _ctx = ctx;

        public async Task AddAsync(ServiceEntity service, CancellationToken ct = default) => await _ctx.Services.AddAsync(service, ct);

        public async Task<IEnumerable<ServiceEntity>> GetAllAsync(CancellationToken ct = default) => await _ctx.Services.ToListAsync(ct);

        public async Task<ServiceEntity?> GetByIdAsync(Guid id, CancellationToken ct = default) => await _ctx.Services.FindAsync(new object[] { id }, ct);

        public void Update(ServiceEntity service) => _ctx.Services.Update(service);

        public async Task SaveChangesAsync(CancellationToken ct = default) => await _ctx.SaveChangesAsync(ct);
    }
}
