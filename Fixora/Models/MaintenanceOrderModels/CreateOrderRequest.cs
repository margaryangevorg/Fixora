using Fixora.DAL.Enums;

namespace Fixora.API.Models.MaintenanceOrderModels;

/// <summary>Used by Manager to create/schedule a new order</summary>
public class CreateOrderRequest
{
    public int ElevatorId { get; set; }
    public string AssignedEngineerId { get; set; } = string.Empty;
    public MaintenanceType MaintenanceType { get; set; }
    public DateTime ScheduledDate { get; set; }
}
