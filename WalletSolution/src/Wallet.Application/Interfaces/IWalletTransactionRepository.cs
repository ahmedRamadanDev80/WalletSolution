using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Core.Entities;

namespace Wallet.Application.Interfaces
{
    public interface IWalletTransactionRepository
    {
        Task AddAsync(WalletTransaction tx, CancellationToken ct = default);
        Task<bool> ExistsByExternalReferenceAsync(Guid walletId, string externalRef, CancellationToken ct = default);
        Task<IReadOnlyList<WalletTransaction>> GetTransactionsAsync(Guid walletId, int skip, int take, CancellationToken ct = default);
        Task<int> CountAsync(Guid walletId, CancellationToken ct = default);

        Task<long> SumAmountByTypeAsync(string type, CancellationToken ct = default);

    }
}
