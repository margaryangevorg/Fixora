using Fixora.API.Models.BuildingModels;
using Fixora.DAL.Constants;
using Fixora.DAL.Entities;
using Fixora.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Fixora.API.Controllers;

[ApiController]
[Route("api/building")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
public class BuildingController : ControllerBase
{
    private readonly IBuildingRepository _buildingRepository;

    public BuildingController(IBuildingRepository buildingRepository)
    {
        _buildingRepository = buildingRepository;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var buildings = await _buildingRepository.GetAllAsync();

        var response = buildings.Select(b => new BuildingResponse
        {
            Id = b.Id,
            Name = b.Name,
            Address = b.Address,
            ElevatorCount = b.Elevators.Count
        });

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var building = await _buildingRepository.GetWithElevatorsAsync(id);
        if (building is null)
            return NotFound($"Building {id} not found.");

        return Ok(new BuildingWithElevatorsResponse
        {
            Id = building.Id,
            Name = building.Name,
            Address = building.Address,
            Elevators = building.Elevators.Select(el => new ElevatorSummary
            {
                Id = el.Id,
                Label = el.Label,
                SerialNumber = el.SerialNumber
            }).ToList()
        });
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] BuildingRequest request)
    {
        var building = new Building
        {
            Name = request.Name,
            Address = request.Address
        };

        await _buildingRepository.AddAsync(building);
        await _buildingRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = building.Id }, new BuildingResponse
        {
            Id = building.Id,
            Name = building.Name,
            Address = building.Address,
            ElevatorCount = 0
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(int id, [FromBody] BuildingRequest request)
    {
        var building = await _buildingRepository.GetByIdAsync(id);
        if (building is null)
            return NotFound($"Building {id} not found.");

        building.Name = request.Name;
        building.Address = request.Address;

        _buildingRepository.Update(building);
        await _buildingRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        var building = await _buildingRepository.GetByIdAsync(id);
        if (building is null)
            return NotFound($"Building {id} not found.");

        _buildingRepository.Delete(building);
        await _buildingRepository.SaveChangesAsync();

        return NoContent();
    }
}
