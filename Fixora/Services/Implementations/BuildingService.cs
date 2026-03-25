using Fixora.API.Models;
using Fixora.API.Models.BuildingModels;
using Fixora.API.Services.Interfaces;
using Fixora.DAL.Entities;
using Fixora.DAL.Repositories.Interfaces;

namespace Fixora.API.Services.Implementations;

public class BuildingService : IBuildingService
{
    private readonly IBuildingRepository _repository;

    public BuildingService(IBuildingRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<BuildingResponse>> GetAllAsync()
    {
        var buildings = await _repository.GetAllAsync();
        return buildings.Select(b => new BuildingResponse
        {
            Id = b.Id,
            Name = b.Name,
            Address = b.Address,
            ElevatorCount = b.Elevators.Count
        });
    }

    public async Task<ServiceResult<BuildingWithElevatorsResponse>> GetByIdAsync(int id)
    {
        var building = await _repository.GetWithElevatorsAsync(id);
        if (building is null)
            return ServiceResult<BuildingWithElevatorsResponse>.NotFound($"Building {id} not found.");

        return ServiceResult<BuildingWithElevatorsResponse>.Ok(new BuildingWithElevatorsResponse
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

    public async Task<BuildingResponse> CreateAsync(BuildingRequest request)
    {
        var building = new Building
        {
            Name = request.Name,
            Address = request.Address
        };

        await _repository.AddAsync(building);
        await _repository.SaveChangesAsync();

        return new BuildingResponse
        {
            Id = building.Id,
            Name = building.Name,
            Address = building.Address,
            ElevatorCount = 0
        };
    }

    public async Task<ServiceResult<bool>> UpdateAsync(int id, BuildingRequest request)
    {
        var building = await _repository.GetByIdAsync(id);
        if (building is null)
            return ServiceResult<bool>.NotFound($"Building {id} not found.");

        building.Name = request.Name;
        building.Address = request.Address;

        _repository.Update(building);
        await _repository.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var building = await _repository.GetByIdAsync(id);
        if (building is null)
            return ServiceResult<bool>.NotFound($"Building {id} not found.");

        _repository.Delete(building);
        await _repository.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
    }
}
