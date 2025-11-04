using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Wallet.Application.DTOs;
using Wallet.Application.Interfaces;
using Wallet.Application.Services;

namespace Wallet.Api.Controllers
{
    [ApiController]
    [Route("api/wallets")]
    [Authorize] // all endpoints require JWT
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly IWalletRepository _walletRepo;

        public WalletController(IWalletService walletService, IWalletRepository walletRepo)
        {
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            _walletRepo = walletRepo ?? throw new ArgumentNullException(nameof(walletRepo));
        }

        // GET api/wallets/balance
        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance(CancellationToken ct)
        {
            if (!TryGetUserIdFromClaims(out var userId)) return Unauthorized();

            var wallet = await _walletRepo.GetByUserIdAsync(userId, ct);
            if (wallet == null) return NotFound(new { message = "Wallet not found" });

            return Ok(new { userId = wallet.UserId, balance = wallet.Balance });
        }

        // POST api/wallets/earn
        [HttpPost("earn")]
        public async Task<IActionResult> Earn([FromBody] EarnRequestDto req, CancellationToken ct)
        {
            if (!TryGetUserIdFromClaims(out var userId)) return Unauthorized();

            // Validate basic input
            if (req == null) return BadRequest("Missing request body");
            if (req.Amount <= 0) return BadRequest("Amount must be > 0");

            var dto = await _walletService.EarnAsync(userId, req.Amount, req.ServiceId, req.ExternalReference, req.Description, ct);
            return Ok(new { userId = dto.UserId, balance = dto.Balance });
        }

        // POST api/wallets/burn
        [HttpPost("burn")]
        public async Task<IActionResult> Burn([FromBody] BurnRequestDto req, CancellationToken ct)
        {
            if (!TryGetUserIdFromClaims(out var userId)) return Unauthorized();

            if (req == null) return BadRequest("Missing request body");
            if (req.Amount <= 0) return BadRequest("Amount must be > 0");

            var dto = await _walletService.BurnAsync(userId, req.Amount, req.ServiceId, req.ExternalReference, req.Description, ct);
            return Ok(new { userId = dto.UserId, balance = dto.Balance });
        }

        // GET api/wallets/transactions?skip=0&take=20
        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions([FromQuery] int skip = 0, [FromQuery] int take = 20, CancellationToken ct = default)
        {
            if (!TryGetUserIdFromClaims(out var userId)) return Unauthorized();

            var result = await _walletService.GetTransactionsAsync(userId, skip, take, ct);

            // return 200 OK even when result.Total == 0 so client gets { items: [], total: 0 }
            // only treat a completely missing result as server-side unexpected condition
            if (result == null)
            {
                // optional: log unexpected null result
                return StatusCode(500, "Failed to fetch transactions.");
            }

            return Ok(result);
        }

        // Helper to read userId from JWT
        private bool TryGetUserIdFromClaims(out Guid userId)
        {
            userId = Guid.Empty;
            var claim = User.FindFirst("userId") ?? User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (claim == null) return false;
            return Guid.TryParse(claim.Value, out userId);
        }

        
    }
}
