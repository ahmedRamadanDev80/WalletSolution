using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Application.Interfaces
{
    public interface IDapperWalletRepository
    {
        Task<long> MutateWithPessimisticLockAsync(Guid userId, long amount, string type, string? externalRef, string? description, CancellationToken ct);
    }
}
