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
    public class ConfigurationRuleService : IConfigurationRuleService
    {
        private readonly IConfigurationRuleRepository _ruleRepo;
        private readonly IServiceRepository _serviceRepo;

        public ConfigurationRuleService(
            IConfigurationRuleRepository ruleRepo,
            IServiceRepository serviceRepo)
        {
            _ruleRepo = ruleRepo;
            _serviceRepo = serviceRepo;
        }

        public async Task<IEnumerable<ConfigurationRuleViewDto>> GetAllAsync(CancellationToken ct = default)
        {
            var rules = await _ruleRepo.GetAllAsync(ct); 

            var serviceIds = rules
                .Select(r => r.ServiceId)
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            var services = serviceIds.Any()
                ? (await _serviceRepo.GetByIdsAsync(serviceIds, ct)).ToList()
                : new List<ServiceEntity>();

            return rules.Select(r => MapToViewDto(r, services)).ToList();
        }

        public async Task<ConfigurationRuleViewDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var r = await _ruleRepo.GetByIdAsync(id, ct);
            if (r == null) return null;

            ServiceEntity? service = null;
            if (r.ServiceId != Guid.Empty)
                service = await _serviceRepo.GetByIdAsync(r.ServiceId, ct);

            return MapToViewDto(r, service == null ? null : new[] { service });
        }

        public async Task<ConfigurationRuleViewDto> CreateAsync(ConfigurationRuleCreateDto dto, CancellationToken ct = default)
        {
            var entity = new ConfigurationRule
            {
                ServiceId = dto.ServiceId,
                RuleType = dto.RuleType,
                PointsPerBaseAmount = dto.PointsPerBaseAmount,
                BaseAmount = dto.BaseAmount,
                IsDefault = dto.IsDefault
            };

            await _ruleRepo.AddAsync(entity, ct);
            await _ruleRepo.SaveChangesAsync(ct);

            ServiceEntity? service = null;
            if (entity.ServiceId != Guid.Empty)
                service = await _serviceRepo.GetByIdAsync(entity.ServiceId, ct);

            return MapToViewDto(entity, service == null ? null : new[] { service });
        }

        public async Task<bool> UpdateAsync(Guid id, ConfigurationRuleUpdateDto dto, CancellationToken ct = default)
        {
            var existing = await _ruleRepo.GetByIdAsync(id, ct);
            if (existing == null) return false;

            if (dto.ServiceId.HasValue && dto.ServiceId.Value != Guid.Empty)
            {
                existing.ServiceId = dto.ServiceId.Value;
            }

            existing.RuleType = dto.RuleType;
            existing.PointsPerBaseAmount = dto.PointsPerBaseAmount;
            existing.BaseAmount = dto.BaseAmount;
            existing.IsDefault = dto.IsDefault;

            _ruleRepo.Update(existing);
            await _ruleRepo.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var exists = await _ruleRepo.GetByIdAsync(id, ct);
            if (exists == null) return false;
            await _ruleRepo.DeleteAsync(id, ct);
            await _ruleRepo.SaveChangesAsync(ct);
            return true;
        }

        private ConfigurationRuleViewDto MapToViewDto(ConfigurationRule r, IEnumerable<ServiceEntity>? services)
        {
            var svc = services?.FirstOrDefault(s => s.Id == r.ServiceId);
            return new ConfigurationRuleViewDto
            {
                Id = r.Id,
                ServiceId = r.ServiceId,
                ServiceName = svc?.Name,
                RuleType = r.RuleType,
                PointsPerBaseAmount = r.PointsPerBaseAmount,
                BaseAmount = r.BaseAmount,
                IsDefault = r.IsDefault
            };
        }

        // overload for single service
        private ConfigurationRuleViewDto MapToViewDto(ConfigurationRule r, ServiceEntity? service)
            => MapToViewDto(r, service == null ? null : new[] { service });
    }
}
