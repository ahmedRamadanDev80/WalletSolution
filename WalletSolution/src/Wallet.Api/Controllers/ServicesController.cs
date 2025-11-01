using Microsoft.AspNetCore.Mvc;
using Wallet.Application.Interfaces;
using Wallet.Core.Entities;

namespace Wallet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceRepository _serviceRepository;

        public ServicesController(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        // GET: /api/services
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceEntity>>> GetAll(CancellationToken ct)
        {
            var services = await _serviceRepository.GetAllAsync(ct);
            return Ok(services);
        }

        // GET: /api/services/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceEntity>> GetById(Guid id, CancellationToken ct)
        {
            var service = await _serviceRepository.GetByIdAsync(id, ct);
            if (service == null)
                return NotFound();

            return Ok(service);
        }

        // POST: /api/services
        [HttpPost]
        public async Task<ActionResult> Create(ServiceEntity service, CancellationToken ct)
        {
            await _serviceRepository.AddAsync(service, ct);
            await _serviceRepository.SaveChangesAsync(ct);

            return CreatedAtAction(nameof(GetById), new { id = service.Id }, service);
        }
    }
}
