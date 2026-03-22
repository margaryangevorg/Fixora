namespace Fixora.DAL.Entities;

public class Elevator
{
    public int Id { get; set; }

    /// <summary>e.g. "L1", "L2" — label shown in the maintenance list</summary>
    public string Label { get; set; } = string.Empty;
    public string? SerialNumber { get; set; }

    public int BuildingId { get; set; }
    public Building Building { get; set; } = null!;

    public ICollection<MaintenanceOrder> MaintenanceOrders { get; set; } = new List<MaintenanceOrder>();
}