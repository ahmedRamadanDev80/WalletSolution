using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Application.DTOs
{
    public class ConfigurationRuleUpdateDto
    {
        public Guid? ServiceId { get; set; }
        public string RuleType { get; set; } = default!;
        public int PointsPerBaseAmount { get; set; }
        public decimal BaseAmount { get; set; }
        public bool IsDefault { get; set; }
    }
}
