using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Application.DTOs;

namespace Wallet.Application.Interfaces
{
    public interface IAdminService
    {
        Task<AdminKpiDto> GetKpisAsync(CancellationToken ct = default);
    }
}
