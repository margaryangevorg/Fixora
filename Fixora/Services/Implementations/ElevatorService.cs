using Fixora.API.Models;
using Fixora.API.Models.ElevatorModels;
using Fixora.API.Services.Interfaces;
using Fixora.DAL.Entities;
using Fixora.DAL.Repositories.Interfaces;

namespace Fixora.API.Services.Implementations;

public class ElevatorService : IElevatorService
{
    private readonly IRepository<Elevator> _elevatorRepository;
    private readonly IBuildingRepository _buildingRepository;

    public ElevatorService(
        IRepository<Elevator> elevatorRepository,
        IBuildingRepository buildingRepository)
    {
        _elevatorRepository = elevatorRepository;
        _buildingRepository = buildingRepository;
    }

    public async Task<IEnumerable<ElevatorResponse>> GetAllAsync()
    {
        var elevators = await _elevatorRepository.GetAllAsync();
        return elevators.Select(el => new ElevatorResponse
        {
            Id = el.Id,
            Label = el.Label,
            SerialNumber = el.SerialNumber,
            BuildingId = el.BuildingId,
            BuildingName = el.Building?.Name ?? string.Empty
        });
    }

    public async Task<ServiceResult<ElevatorResponse>> GetByIdAsync(int id)
    {
        var elevator = await _elevatorRepository.GetByIdAsync(id);
        if (elevator is null)
            return ServiceResult<ElevatorResponse>.NotFound($"Elevator {id} not found.");

        return ServiceResult<ElevatorResponse>.Ok(new ElevatorResponse
        {
            Id = elevator.Id,
            Label = elevator.Label,
            SerialNumber = elevator.SerialNumber,
            BuildingId = elevator.BuildingId,
            BuildingName = elevator.Building?.Name ?? string.Empty
        });
    }

    public async Task<ServiceResult<ElevatorResponse>> CreateAsync(ElevatorRequest request)
    {
        var building = await _buildingRepository.GetByIdAsync(request.BuildingId);
        if (building is null)
            return ServiceResult<ElevatorResponse>.Error($"Building {request.BuildingId} not found.");

        var elevator = new Elevator
        {
            Label = request.Label,
            SerialNumber = request.SerialNumber,
            BuildingId = request.BuildingId
        };

        await _elevatorRepository.AddAsync(elevator);
        await _elevatorRepository.SaveChangesAsync();

        return ServiceResult<ElevatorResponse>.Ok(new ElevatorResponse
        {
            Id = elevator.Id,
            Label = elevator.Label,
            SerialNumber = elevator.SerialNumber,
            BuildingId = elevator.BuildingId,
            BuildingName = building.Name
        });
    }

    public async Task<ServiceResult<bool>> UpdateAsync(int id, ElevatorRequest request)
    {
        var elevator = await _elevatorRepository.GetByIdAsync(id);
        if (elevator is null)
            return ServiceResult<bool>.NotFound($"Elevator {id} not found.");

        var building = await _buildingRepository.GetByIdAsync(request.BuildingId);
        if (building is null)
            return ServiceResult<bool>.Error($"Building {request.BuildingId} not found.");

        elevator.Label = request.Label;
        elevator.SerialNumber = request.SerialNumber;
        elevator.BuildingId = request.BuildingId;

        _elevatorRepository.Update(elevator);
        await _elevatorRepository.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var elevator = await _elevatorRepository.GetByIdAsync(id);
        if (elevator is null)
            return ServiceResult<bool>.NotFound($"Elevator {id} not found.");

        _elevatorRepository.Delete(elevator);
        await _elevatorRepository.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
    }
}
