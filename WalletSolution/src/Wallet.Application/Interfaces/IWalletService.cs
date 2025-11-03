using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Application.DTOs;

namespace Wallet.Application.Interfaces
{
    public interface IWalletService
    {
        Task<WalletDto> EarnAsync(Guid userId, decimal amountMoney, Guid? serviceId, string? externalRef, string? desc, CancellationToken ct = default);
        Task<WalletDto> BurnAsync(Guid userId, decimal amountMoneyOrPoints, Guid? serviceId, string? externalRef, string? desc, CancellationToken ct = default);
        Task<WalletDto?> GetBalanceByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<PagedResult<WalletTransactionDto>> GetTransactionsAsync(Guid userId, int skip, int take, CancellationToken ct = default);
    }
}
