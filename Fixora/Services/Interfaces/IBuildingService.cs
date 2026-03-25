using Fixora.API.Models;
using Fixora.API.Models.BuildingModels;

namespace Fixora.API.Services.Interfaces;

public interface IBuildingService
{
    Task<IEnumerable<BuildingResponse>> GetAllAsync();
    Task<ServiceResult<BuildingWithElevatorsResponse>> GetByIdAsync(int id);
    Task<BuildingResponse> CreateAsync(BuildingRequest request);
    Task<ServiceResult<bool>> UpdateAsync(int id, BuildingRequest request);
    Task<ServiceResult<bool>> DeleteAsync(int id);
}
