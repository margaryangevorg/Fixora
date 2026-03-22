namespace Fixora.API.Models.BuildingModels;

public class BuildingResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int ElevatorCount { get; set; }
}