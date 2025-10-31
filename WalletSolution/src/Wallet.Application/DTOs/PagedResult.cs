using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Application.DTOs
{
    public record PagedResult<T>(IReadOnlyList<T> Items, int Total);
}
