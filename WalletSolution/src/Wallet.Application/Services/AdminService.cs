using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Application.DTOs;
using Wallet.Application.Interfaces;

namespace Wallet.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IWalletRepository _walletRepo;
        private readonly IWalletTransactionRepository _txRepo;

        public AdminService( IWalletRepository walletRepo, IWalletTransactionRepository txRepo)
        {
            _walletRepo = walletRepo;
            _txRepo = txRepo;
        }

        public async Task<AdminKpiDto> GetKpisAsync(CancellationToken ct = default)
        {

            var totalEarned = await  _txRepo.SumAmountByTypeAsync("EARN", ct);

            var totalBurned = await _txRepo.SumAmountByTypeAsync("BURN", ct);

            var activeWallets = await _walletRepo.CountActiveAsync(ct);

            return new AdminKpiDto
            {
                TotalEarned = totalEarned,
                TotalBurned = totalBurned,
                ActiveWallets = activeWallets
            };
        }
    }
}
