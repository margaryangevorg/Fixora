namespace Fixora.API.Models.MaintenanceOrderModels;

/// <summary>Used by Engineer to submit completion of a job</summary>
public class CompleteOrderRequest
{
    public bool IssueDetected { get; set; }
    public bool VisualCheckDone { get; set; }
    public bool AdjustmentDone { get; set; }
    public bool CleaningDone { get; set; }
    public string? ShortDescription { get; set; }
    public DateTime CompletionDate { get; set; }
}
