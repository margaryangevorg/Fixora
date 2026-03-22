using Microsoft.AspNetCore.Identity;

namespace Fixora.DAL.Entities;

public class AppUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public ICollection<MaintenanceOrder> AssignedOrders { get; set; } = [];
}
