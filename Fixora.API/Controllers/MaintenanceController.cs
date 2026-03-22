using Fixora.API.Models.InputModels;
using Fixora.API.Models.MaintenanceOrderModels;
using Fixora.DAL.Constants;
using Fixora.DAL.Entities;
using Fixora.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Fixora.API.Controllers;

[ApiController]
[Route("api/maintenance")]
[Authorize]
public class MaintenanceController : ControllerBase
{
    private readonly IMaintenanceOrderRepository _orderRepository;
    private readonly IRepository<Elevator> _elevatorRepository;

    public MaintenanceController(
        IMaintenanceOrderRepository orderRepository,
        IRepository<Elevator> elevatorRepository)
    {
        _orderRepository = orderRepository;
        _elevatorRepository = elevatorRepository;
    }

    // GET /maintenance/elevators?untilDate=2025-06-01
    // All orders scheduled up to a given date — Admin/Manager overview
    [HttpGet("elevators")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public async Task<IActionResult> GetElevatorsUnderMaintenance([FromQuery] DateTime untilDate)
    {
        var orders = await _orderRepository.GetOrdersUntilDateAsync(untilDate);
        return Ok(orders.Select(MapToResponse));
    }

    // GET /maintenance/monthly?engineerId=xxx&year=2025&month=6
    // Monthly list — Engineer sees their own, Manager/Admin can query any
    [HttpGet("monthly")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager},{Roles.MaintenanceEngineer}")]
    public async Task<IActionResult> GetMonthlyList(
        [FromQuery] string engineerId,
        [FromQuery] int year,
        [FromQuery] int month)
    {
        var orders = await _orderRepository.GetMonthlyOrdersByEngineerAsync(engineerId, year, month);
        return Ok(orders.Select(MapToResponse));
    }

    // GET /maintenance/unscheduled
    // Unscheduled tab — Admin/Manager only
    [HttpGet("unscheduled")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public async Task<IActionResult> GetUnscheduled()
    {
        var orders = await _orderRepository.GetUnscheduledOrdersAsync();
        return Ok(orders.Select(MapToResponse));
    }

    // POST /maintenance
    // Manager creates and assigns a new maintenance order
    [HttpPost]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var elevator = await _elevatorRepository.GetByIdAsync(request.ElevatorId);
        if (elevator is null)
            return BadRequest($"Elevator {request.ElevatorId} not found.");

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

        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order.Id);
    }

    // GET /maintenance/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order is null)
            return NotFound($"Order {id} not found.");

        return Ok(MapToResponse(order));
    }

    // PUT /maintenance/{id}/complete
    // Engineer submits the completion form after finishing a job
    [HttpPut("{id}/complete")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager},{Roles.MaintenanceEngineer}")]
    public async Task<IActionResult> CompleteOrder(int id, [FromBody] CompleteOrderRequest request)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order is null)
            return NotFound($"Order {id} not found.");

        if (order.IsCompleted)
            return BadRequest("This order is already completed.");

        order.IsCompleted = true;
        order.CompletionDate = request.CompletionDate;
        order.IssueDetected = request.IssueDetected;
        order.VisualCheckDone = request.VisualCheckDone;
        order.AdjustmentDone = request.AdjustmentDone;
        order.CleaningDone = request.CleaningDone;
        order.ShortDescription = request.ShortDescription;

        _orderRepository.Update(order);
        await _orderRepository.SaveChangesAsync();

        return NoContent();
    }

    // DELETE /maintenance/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order is null)
            return NotFound($"Order {id} not found.");

        _orderRepository.Delete(order);
        await _orderRepository.SaveChangesAsync();

        return NoContent();
    }

    // Maps entity to response DTO — keeps entity out of the API contract
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
