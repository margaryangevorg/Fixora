using Fixora.API.Models.ElevatorModels;
using Fixora.API.Models.InputModels;
using Fixora.DAL.Constants;
using Fixora.DAL.Entities;
using Fixora.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Fixora.Controllers;

[ApiController]
[Route("api/elevator")]
[Authorize]
public class ElevatorController : ControllerBase
{
    private readonly IRepository<Elevator> _elevatorRepository;
    private readonly IBuildingRepository _buildingRepository;

    public ElevatorController(
        IRepository<Elevator> elevatorRepository,
        IBuildingRepository buildingRepository)
    {
        _elevatorRepository = elevatorRepository;
        _buildingRepository = buildingRepository;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var elevators = await _elevatorRepository.GetAllAsync();

        var response = elevators.Select(el => new ElevatorResponse
        {
            Id = el.Id,
            Label = el.Label,
            SerialNumber = el.SerialNumber,
            BuildingId = el.BuildingId,
            BuildingName = el.Building?.Name ?? string.Empty
        });

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var elevator = await _elevatorRepository.GetByIdAsync(id);
        if (elevator is null)
            return NotFound($"Elevator {id} not found.");

        return Ok(new ElevatorResponse
        {
            Id = elevator.Id,
            Label = elevator.Label,
            SerialNumber = elevator.SerialNumber,
            BuildingId = elevator.BuildingId,
            BuildingName = elevator.Building?.Name ?? string.Empty
        });
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public async Task<IActionResult> Create([FromBody] ElevatorRequest request)
    {
        // Verify the building exists before creating the elevator
        var building = await _buildingRepository.GetByIdAsync(request.BuildingId);
        if (building is null)
            return BadRequest($"Building {request.BuildingId} not found.");

        var elevator = new Elevator
        {
            Label = request.Label,
            SerialNumber = request.SerialNumber,
            BuildingId = request.BuildingId
        };

        await _elevatorRepository.AddAsync(elevator);
        await _elevatorRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = elevator.Id }, new ElevatorResponse
        {
            Id = elevator.Id,
            Label = elevator.Label,
            SerialNumber = elevator.SerialNumber,
            BuildingId = elevator.BuildingId,
            BuildingName = building.Name
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public async Task<IActionResult> Update(int id, [FromBody] ElevatorRequest request)
    {
        var elevator = await _elevatorRepository.GetByIdAsync(id);
        if (elevator is null)
            return NotFound($"Elevator {id} not found.");

        var building = await _buildingRepository.GetByIdAsync(request.BuildingId);
        if (building is null)
            return BadRequest($"Building {request.BuildingId} not found.");

        elevator.Label = request.Label;
        elevator.SerialNumber = request.SerialNumber;
        elevator.BuildingId = request.BuildingId;

        _elevatorRepository.Update(elevator);
        await _elevatorRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        var elevator = await _elevatorRepository.GetByIdAsync(id);
        if (elevator is null)
            return NotFound($"Elevator {id} not found.");

        _elevatorRepository.Delete(elevator);
        await _elevatorRepository.SaveChangesAsync();

        return NoContent();
    }
}