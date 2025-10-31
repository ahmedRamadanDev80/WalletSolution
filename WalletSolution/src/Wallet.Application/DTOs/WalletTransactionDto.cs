using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Application.DTOs
{
    public record WalletTransactionDto(
        Guid Id,
        string Type,
        long Amount,
        long BalanceAfter,
        string? Description,
        string? ExternalReference,
        DateTime CreatedAt
    );
}
