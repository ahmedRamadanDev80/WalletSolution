using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Application.DTOs
{
    public record EarnRequestDto(decimal Amount, Guid? ServiceId, string? ExternalReference, string? Description);
}
