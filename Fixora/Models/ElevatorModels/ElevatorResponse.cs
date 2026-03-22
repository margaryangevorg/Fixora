namespace Fixora.API.Models.ElevatorModels;

public class ElevatorResponse
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string? SerialNumber { get; set; }
    public int BuildingId { get; set; }
    public string BuildingName { get; set; } = string.Empty;
}