using Fixora.API.Models.ElevatorModels;
using Fixora.API.Services.Interfaces;
using Fixora.DAL.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixora.Controllers;

[ApiController]
[Route("api/elevator")]
[Authorize]
public class ElevatorController : ControllerBase
{
    private readonly IElevatorService _elevatorService;

    public ElevatorController(IElevatorService elevatorService)
    {
        _elevatorService = elevatorService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _elevatorService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _elevatorService.GetByIdAsync(id);
        if (result.IsNotFound) return NotFound(result.ErrorMessage);
        return Ok(result.Data);
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public async Task<IActionResult> Create([FromBody] ElevatorRequest request)
    {
        var result = await _elevatorService.CreateAsync(request);
        if (!result.IsSuccess) return BadRequest(result.ErrorMessage);
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public async Task<IActionResult> Update(int id, [FromBody] ElevatorRequest request)
    {
        var result = await _elevatorService.UpdateAsync(id, request);
        if (result.IsNotFound) return NotFound(result.ErrorMessage);
        if (!result.IsSuccess) return BadRequest(result.ErrorMessage);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _elevatorService.DeleteAsync(id);
        if (result.IsNotFound) return NotFound(result.ErrorMessage);
        return NoContent();
    }
}
