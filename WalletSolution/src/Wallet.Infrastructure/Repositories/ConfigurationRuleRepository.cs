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
    public class ConfigurationRuleRepository : IConfigurationRuleRepository
    {
        private readonly WalletDbContext _ctx;
        public ConfigurationRuleRepository(WalletDbContext ctx) => _ctx = ctx;

        public async Task AddAsync(ConfigurationRule rule, CancellationToken ct = default) => await _ctx.ConfigurationRules.AddAsync(rule, ct);

        public async Task<ConfigurationRule?> GetByIdAsync(Guid id, CancellationToken ct = default) => await _ctx.ConfigurationRules.FindAsync(new object[] { id }, ct);

        public async Task<ConfigurationRule?> GetByServiceAndTypeAsync(Guid serviceId, string ruleType, CancellationToken ct = default) =>
            await _ctx.ConfigurationRules.FirstOrDefaultAsync(r => r.ServiceId == serviceId && r.RuleType == ruleType, ct);

        public async Task<ConfigurationRule?> GetDefaultRuleAsync(string ruleType, CancellationToken ct = default) =>
            await _ctx.ConfigurationRules.FirstOrDefaultAsync(r => r.IsDefault && r.RuleType == ruleType, ct);

        public void Update(ConfigurationRule rule) => _ctx.ConfigurationRules.Update(rule);

        public async Task SaveChangesAsync(CancellationToken ct = default) => await _ctx.SaveChangesAsync(ct);
    }
}
