namespace Fixora.DAL.Entities;

public class Building
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    public ICollection<Elevator> Elevators { get; set; } = new List<Elevator>();
}