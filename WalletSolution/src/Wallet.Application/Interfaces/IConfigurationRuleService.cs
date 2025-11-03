using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Application.DTOs;

namespace Wallet.Application.Interfaces
{
    public interface IConfigurationRuleService
    {
        Task<IEnumerable<ConfigurationRuleViewDto>> GetAllAsync(CancellationToken ct = default);
        Task<ConfigurationRuleViewDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<ConfigurationRuleViewDto> CreateAsync(ConfigurationRuleCreateDto dto, CancellationToken ct = default);
        Task<bool> UpdateAsync(Guid id, ConfigurationRuleUpdateDto dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
