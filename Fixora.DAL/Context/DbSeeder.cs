using Fixora.DAL.Constants;
using Microsoft.AspNetCore.Identity;

namespace Fixora.DAL.Context;

public static class DbSeeder
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = [Roles.Admin, Roles.Manager, Roles.MaintenanceEngineer];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}
