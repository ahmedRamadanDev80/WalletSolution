using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Core.Entities
{
    public class ConfigurationRule
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ServiceId { get; set; }        
        public string RuleType { get; set; } = "EARNING"; 
        public int PointsPerBaseAmount { get; set; } = 1; 
        public decimal BaseAmount { get; set; } = 1m; 
        public bool IsDefault { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
