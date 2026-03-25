using Fixora.API.Models;
using Fixora.API.Models.MaintenanceOrderModels;

namespace Fixora.API.Services.Interfaces;

public interface IMaintenanceService
{
    Task<IEnumerable<MaintenanceOrderResponse>> GetOrdersUntilDateAsync(DateTime untilDate);
    Task<IEnumerable<MaintenanceOrderResponse>> GetMonthlyListAsync(string engineerId, int year, int month);
    Task<IEnumerable<MaintenanceOrderResponse>> GetUnscheduledAsync();
    Task<ServiceResult<int>> CreateOrderAsync(CreateOrderRequest request);
    Task<ServiceResult<MaintenanceOrderResponse>> GetByIdAsync(int id);
    Task<ServiceResult<bool>> CompleteOrderAsync(int id, CompleteOrderRequest request);
    Task<ServiceResult<bool>> DeleteAsync(int id);
}
