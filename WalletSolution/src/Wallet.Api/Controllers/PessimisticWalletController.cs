using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Wallet.Application.Interfaces;

namespace Wallet.Api.Controllers
{
    [ApiController]
    [Route("api/pessimistic/wallets")]
    public class PessimisticWalletController : ControllerBase
    {
        private readonly IDapperWalletRepository _repo;

        public PessimisticWalletController(IDapperWalletRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("{userId:guid}/earn")]
        public async Task<IActionResult> Earn(Guid userId, [FromBody] PessimisticMutateRequest req, CancellationToken ct)
        {
            int attempts = 0;
            while (true)
            {
                attempts++;
                try
                {
                    var newBalance = await _repo.MutateWithPessimisticLockAsync(userId, req.Amount, "EARN", req.ExternalReference, req.Description, ct);
                    return Ok(new { userId, balance = newBalance });
                }
                catch (SqlException ex) when (ex.Number == 1205 && attempts < 3) // deadlock -> retry
                {
                    await Task.Delay(50 * attempts, ct);
                    continue;
                }
                catch (SqlException ex) when (ex.Number == 1205)
                {
                    return StatusCode(409, new { message = "Deadlock occurred, try again." });
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }
        }

        [HttpPost("{userId:guid}/burn")]
        public async Task<IActionResult> Burn(Guid userId, [FromBody] PessimisticMutateRequest req, CancellationToken ct)
        {
            int attempts = 0;
            while (true)
            {
                attempts++;
                try
                {
                    var newBalance = await _repo.MutateWithPessimisticLockAsync(userId, req.Amount, "BURN", req.ExternalReference, req.Description, ct);
                    return Ok(new { userId, balance = newBalance });
                }
                catch (SqlException ex) when (ex.Number == 1205 && attempts < 3)
                {
                    await Task.Delay(50 * attempts, ct);
                    continue;
                }
                catch (SqlException ex) when (ex.Number == 1205)
                {
                    return StatusCode(409, new { message = "Deadlock occurred, try again." });
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }
        }

        public record PessimisticMutateRequest(long Amount, string? ExternalReference, string? Description);
    }
}
