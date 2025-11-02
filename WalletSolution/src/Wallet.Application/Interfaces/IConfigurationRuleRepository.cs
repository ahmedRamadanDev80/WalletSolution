using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Core.Entities;

namespace Wallet.Application.Interfaces
{
    public interface IConfigurationRuleRepository
    {
        Task<ConfigurationRule?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<ConfigurationRule?> GetByServiceAndTypeAsync(Guid serviceId, string ruleType, CancellationToken ct = default);
        Task<ConfigurationRule?> GetDefaultRuleAsync(string ruleType, CancellationToken ct = default);
        Task AddAsync(ConfigurationRule rule, CancellationToken ct = default);
        void Update(ConfigurationRule rule);
        Task<IEnumerable<ConfigurationRule>> GetAllAsync(CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
