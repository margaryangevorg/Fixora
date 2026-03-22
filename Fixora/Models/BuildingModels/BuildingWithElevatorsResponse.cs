namespace Fixora.API.Models.BuildingModels;

public class BuildingWithElevatorsResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public List<ElevatorSummary> Elevators { get; set; } = new();
}
