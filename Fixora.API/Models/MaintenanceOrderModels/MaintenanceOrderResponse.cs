namespace Fixora.API.Models.MaintenanceOrderModels;

public class MaintenanceOrderResponse
{
    public int Id { get; set; }
    public int ElevatorId { get; set; }
    public string ElevatorLabel { get; set; } = string.Empty;
    public string BuildingName { get; set; } = string.Empty;
    public string AssignedEngineerId { get; set; } = string.Empty;
    public string AssignedEngineerName { get; set; } = string.Empty;
    public string MaintenanceType { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public bool IsCompleted { get; set; }
    public bool IssueDetected { get; set; }
    public bool VisualCheckDone { get; set; }
    public bool AdjustmentDone { get; set; }
    public bool CleaningDone { get; set; }
    public string? ShortDescription { get; set; }
}