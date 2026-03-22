using Fixora.DAL.Enums;

namespace Fixora.DAL.Entities;

public class MaintenanceOrder
{
    public int Id { get; set; }

    public int ElevatorId { get; set; }
    public Elevator Elevator { get; set; } = null!;

    /// <summary>Assigned by Manager</summary>
    public string AssignedEngineerId { get; set; } = string.Empty;
    public AppUser AssignedEngineer { get; set; } = null!;

    public MaintenanceType MaintenanceType { get; set; }

    /// <summary>Set by Manager when scheduling the job</summary>
    public DateTime ScheduledDate { get; set; }

    /// <summary>Filled by Engineer after completing the job</summary>
    public DateTime? CompletionDate { get; set; }
    public bool IsCompleted { get; set; }

    // Checklist from the maintenance form
    public bool IssueDetected { get; set; }
    public bool VisualCheckDone { get; set; }
    public bool AdjustmentDone { get; set; }
    public bool CleaningDone { get; set; }

    public string? ShortDescription { get; set; }
}