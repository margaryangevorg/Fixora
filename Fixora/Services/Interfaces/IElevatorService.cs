using Fixora.API.Models;
using Fixora.API.Models.ElevatorModels;

namespace Fixora.API.Services.Interfaces;

public interface IElevatorService
{
    Task<IEnumerable<ElevatorResponse>> GetAllAsync();
    Task<ServiceResult<ElevatorResponse>> GetByIdAsync(int id);
    Task<ServiceResult<ElevatorResponse>> CreateAsync(ElevatorRequest request);
    Task<ServiceResult<bool>> UpdateAsync(int id, ElevatorRequest request);
    Task<ServiceResult<bool>> DeleteAsync(int id);
}
