using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Infrastructure.Dapper
{
    public class DapperOptions
    {
        /// <summary>
        /// Seconds to use as command timeout for Dapper commands.
        /// </summary>
        public int CommandTimeoutSeconds { get; set; } = 30;
    }
}
