using Fixora.DAL.Entities;

namespace Fixora.DAL.Repositories.Interfaces;

public interface IMaintenanceOrderRepository : IRepository<MaintenanceOrder>
{
    /// <summary>
    /// Powers the "Full Monthly Maintenance List" screen.
    /// </summary>
    Task<IEnumerable<MaintenanceOrder>> GetMonthlyOrdersByEngineerAsync(string engineerId, int year, int month);

    /// <summary>
    /// Powers the MaintenanceController "elevators until date" endpoint.
    /// </summary>
    Task<IEnumerable<MaintenanceOrder>> GetOrdersUntilDateAsync(DateTime untilDate);

    /// <summary>
    /// Returns all unscheduled pending orders — shown in Unscheduled tab.
    /// </summary>
    Task<IEnumerable<MaintenanceOrder>> GetUnscheduledOrdersAsync();
}