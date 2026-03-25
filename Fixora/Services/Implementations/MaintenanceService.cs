using Fixora.API.Models;
using Fixora.API.Models.MaintenanceOrderModels;
using Fixora.API.Services.Interfaces;
using Fixora.DAL.Entities;
using Fixora.DAL.Repositories.Interfaces;

namespace Fixora.API.Services.Implementations;

public class MaintenanceService : IMaintenanceService
{
    private readonly IMaintenanceOrderRepository _orderRepository;
    private readonly IRepository<Elevator> _elevatorRepository;

    public MaintenanceService(
        IMaintenanceOrderRepository orderRepository,
        IRepository<Elevator> elevatorRepository)
    {
        _orderRepository = orderRepository;
        _elevatorRepository = elevatorRepository;
    }

    public async Task<IEnumerable<MaintenanceOrderResponse>> GetOrdersUntilDateAsync(DateTime untilDate)
    {
        var orders = await _orderRepository.GetOrdersUntilDateAsync(untilDate);
        return orders.Select(MapToResponse);
    }

    public async Task<IEnumerable<MaintenanceOrderResponse>> GetMonthlyListAsync(string engineerId, int year, int month)
    {
        var orders = await _orderRepository.GetMonthlyOrdersByEngineerAsync(engineerId, year, month);
        return orders.Select(MapToResponse);
    }

    public async Task<IEnumerable<MaintenanceOrderResponse>> GetUnscheduledAsync()
    {
        var orders = await _orderRepository.GetUnscheduledOrdersAsync();
        return orders.Select(MapToResponse);
    }

    public async Task<ServiceResult<int>> CreateOrderAsync(CreateOrderRequest request)
    {
        var elevator = await _elevatorRepository.GetByIdAsync(request.ElevatorId);
        if (elevator is null)
            return ServiceResult<int>.Error($"Elevator {request.ElevatorId} not found.");

        var order = new MaintenanceOrder
        {
            ElevatorId = request.ElevatorId,
            AssignedEngineerId = request.AssignedEngineerId,
            MaintenanceType = request.MaintenanceType,
            ScheduledDate = request.ScheduledDate,
            IsCompleted = false
        };

        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();

        return ServiceResult<int>.Ok(order.Id);
    }

    public async Task<ServiceResult<MaintenanceOrderResponse>> GetByIdAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order is null)
            return ServiceResult<MaintenanceOrderResponse>.NotFound($"Order {id} not found.");

        return ServiceResult<MaintenanceOrderResponse>.Ok(MapToResponse(order));
    }

    public async Task<ServiceResult<bool>> CompleteOrderAsync(int id, CompleteOrderRequest request)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order is null)
            return ServiceResult<bool>.NotFound($"Order {id} not found.");

        if (order.IsCompleted)
            return ServiceResult<bool>.Error("This order is already completed.");

        order.IsCompleted = true;
        order.CompletionDate = request.CompletionDate;
        order.IssueDetected = request.IssueDetected;
        order.VisualCheckDone = request.VisualCheckDone;
        order.AdjustmentDone = request.AdjustmentDone;
        order.CleaningDone = request.CleaningDone;
        order.ShortDescription = request.ShortDescription;

        _orderRepository.Update(order);
        await _orderRepository.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order is null)
            return ServiceResult<bool>.NotFound($"Order {id} not found.");

        _orderRepository.Delete(order);
        await _orderRepository.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
    }

    private static MaintenanceOrderResponse MapToResponse(MaintenanceOrder o) => new()
    {
        Id = o.Id,
        ElevatorId = o.ElevatorId,
        ElevatorLabel = o.Elevator?.Label ?? string.Empty,
        BuildingName = o.Elevator?.Building?.Name ?? string.Empty,
        AssignedEngineerId = o.AssignedEngineerId,
        AssignedEngineerName = o.AssignedEngineer is null
            ? string.Empty
            : $"{o.AssignedEngineer.FirstName} {o.AssignedEngineer.LastName}",
        MaintenanceType = o.MaintenanceType.ToString(),
        ScheduledDate = o.ScheduledDate,
        CompletionDate = o.CompletionDate,
        IsCompleted = o.IsCompleted,
        IssueDetected = o.IssueDetected,
        VisualCheckDone = o.VisualCheckDone,
        AdjustmentDone = o.AdjustmentDone,
        CleaningDone = o.CleaningDone,
        ShortDescription = o.ShortDescription
    };
}
