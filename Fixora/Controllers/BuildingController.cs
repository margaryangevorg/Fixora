using Fixora.API.Models.BuildingModels;
using Fixora.API.Services.Interfaces;
using Fixora.DAL.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixora.API.Controllers;

[ApiController]
[Route("api/building")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
public class BuildingController : ControllerBase
{
    private readonly IBuildingService _buildingService;

    public BuildingController(IBuildingService buildingService)
    {
        _buildingService = buildingService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _buildingService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _buildingService.GetByIdAsync(id);
        if (result.IsNotFound) return NotFound(result.ErrorMessage);
        return Ok(result.Data);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] BuildingRequest request)
    {
        var response = await _buildingService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(int id, [FromBody] BuildingRequest request)
    {
        var result = await _buildingService.UpdateAsync(id, request);
        if (result.IsNotFound) return NotFound(result.ErrorMessage);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _buildingService.DeleteAsync(id);
        if (result.IsNotFound) return NotFound(result.ErrorMessage);
        return NoContent();
    }
}
