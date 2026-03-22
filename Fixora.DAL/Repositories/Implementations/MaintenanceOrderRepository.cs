using Fixora.DAL.Context;
using Fixora.DAL.Entities;
using Fixora.DAL.Enums;
using Fixora.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fixora.DAL.Repositories.Implementations;

public class MaintenanceOrderRepository : Repository<MaintenanceOrder>, IMaintenanceOrderRepository
{
    public MaintenanceOrderRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<MaintenanceOrder>> GetMonthlyOrdersByEngineerAsync(
        string engineerId, int year, int month)
    {
        return await _context.MaintenanceOrders
            .Include(o => o.Elevator)
                .ThenInclude(el => el.Building)
            .Where(o => o.AssignedEngineerId == engineerId
                     && o.ScheduledDate.Year == year
                     && o.ScheduledDate.Month == month)
            .OrderBy(o => o.Elevator.Building.Name)
            .ThenBy(o => o.Elevator.Label)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<MaintenanceOrder>> GetOrdersUntilDateAsync(DateTime untilDate)
    {
        return await _context.MaintenanceOrders
            .Include(o => o.Elevator)
                .ThenInclude(el => el.Building)
            .Where(o => o.ScheduledDate <= untilDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<MaintenanceOrder>> GetUnscheduledOrdersAsync()
    {
        return await _context.MaintenanceOrders
            .Include(o => o.Elevator)
                .ThenInclude(el => el.Building)
            .Where(o => o.MaintenanceType == MaintenanceType.Unscheduled && !o.IsCompleted)
            .AsNoTracking()
            .ToListAsync();
    }
}