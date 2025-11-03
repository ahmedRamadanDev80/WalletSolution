using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Application.DTOs
{
    public record BurnRequestDto(decimal Amount, string? ExternalReference, string? Description,Guid? ServiceId);
}
