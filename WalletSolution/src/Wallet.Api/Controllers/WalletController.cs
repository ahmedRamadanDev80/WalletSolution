using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using Wallet.Application.Services;
using Wallet.Application.Interfaces;
using Wallet.Application.DTOs;

namespace Wallet.Api.Controllers
{
    [ApiController]
    [Route("api/wallets")]
    public class WalletController : ControllerBase
    {
        private readonly WalletService _walletService;
        private readonly IWalletRepository _walletRepo;

        public WalletController(WalletService walletService, IWalletRepository walletRepo)
        {
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            _walletRepo = walletRepo ?? throw new ArgumentNullException(nameof(walletRepo));
        }

        // GET api/wallets/{userId}/balance
        [HttpGet("{userId:guid}/balance")]
        public async Task<IActionResult> GetBalance(Guid userId, CancellationToken ct)
        {
            var wallet = await _walletRepo.GetByUserIdAsync(userId, ct);
            if (wallet == null) return NotFound(new { message = "Wallet not found" });
            return Ok(new { userId = wallet.UserId, balance = wallet.Balance });
        }

        // POST api/wallets/{userId}/earn
        [HttpPost("{userId:guid}/earn")]
        public async Task<IActionResult> Earn(Guid userId, [FromBody] EarnRequest req, CancellationToken ct)
        {
            var dto = await _walletService.EarnAsync(userId, req.Amount, req.ExternalReference, req.Description, ct);
            return Ok(new { userId = dto.UserId, balance = dto.Balance });

        }

        // POST api/wallets/{userId}/burn
        [HttpPost("{userId:guid}/burn")]
        public async Task<IActionResult> Burn(Guid userId, [FromBody] BurnRequest req, CancellationToken ct)
        {
            var dto = await _walletService.BurnAsync(userId, req.Amount, req.ExternalReference, req.Description, ct);
            return Ok(new { userId = dto.UserId, balance = dto.Balance });
        }

        [HttpGet("{userId:guid}/transactions")]
        public async Task<IActionResult> GetTransactions(Guid userId, [FromQuery] int skip = 0, [FromQuery] int take = 20, CancellationToken ct = default)
        {
            var result = await _walletService.GetTransactionsAsync(userId, skip, take, ct);

            // If you prefer 404 when wallet missing and no transactions:
            if (result.Total == 0) return NotFound();

            return Ok(result);
        }

    }
}
