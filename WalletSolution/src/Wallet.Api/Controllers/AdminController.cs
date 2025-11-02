using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Wallet.Application.Interfaces;

namespace Wallet.Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("kpis")]
        public async Task<IActionResult> GetKpis(CancellationToken ct)
        {
            var kpis = await _adminService.GetKpisAsync(ct);
            return Ok(new
            {
                totalEarned = kpis.TotalEarned,
                totalBurned = kpis.TotalBurned,
                activeWallets = kpis.ActiveWallets
            });
        }
    }
}
