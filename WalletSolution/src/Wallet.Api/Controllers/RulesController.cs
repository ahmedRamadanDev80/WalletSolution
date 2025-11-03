using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wallet.Application.DTOs;
using Wallet.Application.Interfaces;
using Wallet.Core.Entities;

[ApiController]
[Route("api/[controller]")]
public class RulesController : ControllerBase
{
    private readonly IConfigurationRuleService _ruleService;

    public RulesController(IConfigurationRuleService ruleService)
    {
        _ruleService = ruleService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ConfigurationRuleViewDto>>> GetAll(CancellationToken ct)
    {
        var dtos = await _ruleService.GetAllAsync(ct);
        return Ok(dtos);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ConfigurationRuleViewDto>> Get(Guid id, CancellationToken ct)
    {
        var dto = await _ruleService.GetByIdAsync(id, ct);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ConfigurationRuleCreateDto dto, CancellationToken ct)
    {
        var created = await _ruleService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ConfigurationRuleUpdateDto dto, CancellationToken ct)
    {
        var updated = await _ruleService.UpdateAsync(id, dto, ct);
        if (!updated) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _ruleService.DeleteAsync(id, ct);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
