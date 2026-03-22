namespace Fixora.API.Models.ElevatorModels;

public class ElevatorRequest
{
    public string Label { get; set; } = string.Empty;
    public string? SerialNumber { get; set; }
    public int BuildingId { get; set; }
}
