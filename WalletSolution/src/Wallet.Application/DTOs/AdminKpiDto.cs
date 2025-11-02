using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Application.DTOs
{
    public class AdminKpiDto
    {
        public long TotalEarned { get; set; }
        public long TotalBurned { get; set; }
        public int ActiveWallets { get; set; }
    }
}
