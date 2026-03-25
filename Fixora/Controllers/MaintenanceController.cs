using Fixora.API.Models.MaintenanceOrderModels;
using Fixora.API.Services.Interfaces;
using Fixora.DAL.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixora.API.Controllers;

[ApiController]
[Route("api/maintenance")]
[Authorize]
public class MaintenanceController : ControllerBase
{
    private readonly IMaintenanceService _maintenanceService;

    public MaintenanceController(IMaintenanceService maintenanceService)
    {
        _maintenanceService = maintenanceService;
    }

    [HttpGet("elevators")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public async Task<IActionResult> GetElevatorsUnderMaintenance([FromQuery] DateTime untilDate)
    {
        return Ok(await _maintenanceService.GetOrdersUntilDateAsync(untilDate));
    }

    [HttpGet("monthly")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager},{Roles.MaintenanceEngineer}")]
    public async Task<IActionResult> GetMonthlyList(
        [FromQuery] string engineerId,
        [FromQuery] int year,
        [FromQuery] int month)
    {
        return Ok(await _maintenanceService.GetMonthlyListAsync(engineerId, year, month));
    }

    [HttpGet("unscheduled")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public async Task<IActionResult> GetUnscheduled()
    {
        return Ok(await _maintenanceService.GetUnscheduledAsync());
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var result = await _maintenanceService.CreateOrderAsync(request);
        if (!result.IsSuccess) return BadRequest(result.ErrorMessage);
        return CreatedAtAction(nameof(GetById), new { id = result.Data }, result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _maintenanceService.GetByIdAsync(id);
        if (result.IsNotFound) return NotFound(result.ErrorMessage);
        return Ok(result.Data);
    }

    [HttpPut("{id}/complete")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager},{Roles.MaintenanceEngineer}")]
    public async Task<IActionResult> CompleteOrder(int id, [FromBody] CompleteOrderRequest request)
    {
        var result = await _maintenanceService.CompleteOrderAsync(id, request);
        if (result.IsNotFound) return NotFound(result.ErrorMessage);
        if (!result.IsSuccess) return BadRequest(result.ErrorMessage);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _maintenanceService.DeleteAsync(id);
        if (result.IsNotFound) return NotFound(result.ErrorMessage);
        return NoContent();
    }
}
