using Fixora.DAL.Context;
using Fixora.DAL.Entities;
using Fixora.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fixora.DAL.Repositories.Implementations;

public class BuildingRepository : Repository<Building>, IBuildingRepository
{
    public BuildingRepository(AppDbContext context) : base(context) { }

    public async Task<Building?> GetWithElevatorsAsync(int buildingId)
    {
        return await _context.Buildings
            .Include(b => b.Elevators)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == buildingId);
    }
}