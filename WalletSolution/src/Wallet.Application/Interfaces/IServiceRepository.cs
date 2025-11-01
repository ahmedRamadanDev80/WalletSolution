using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Core.Entities;

namespace Wallet.Application.Interfaces
{
    public interface IServiceRepository
    {
        Task<ServiceEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<ServiceEntity>> GetAllAsync(CancellationToken ct = default);
        Task AddAsync(ServiceEntity service, CancellationToken ct = default);
        void Update(ServiceEntity service);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
