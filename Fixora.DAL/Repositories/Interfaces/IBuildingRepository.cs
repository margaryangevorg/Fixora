using Fixora.DAL.Entities;

namespace Fixora.DAL.Repositories.Interfaces;

public interface IBuildingRepository : IRepository<Building>
{
    /// <summary>Returns building with all its elevators included.</summary>
    Task<Building?> GetWithElevatorsAsync(int buildingId);
}