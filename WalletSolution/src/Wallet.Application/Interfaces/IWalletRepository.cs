using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Core.Entities;

namespace Wallet.Application.Interfaces
{
    public interface IWalletRepository
    {
        Task<WalletEntity?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<WalletEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task CreateAsync(WalletEntity wallet, CancellationToken ct = default);
        Task<int> CountActiveAsync(CancellationToken ct = default);
        void Update(WalletEntity wallet);
    }
}
