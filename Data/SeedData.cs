using AcxiomCRM.Models;
using Microsoft.AspNetCore.Identity;

namespace AcxiomCRM.Data;

public static class SeedData
{
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string SalesExecutive = "SalesExecutive";

    public static async Task InitializeAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in new[] { Admin, Manager, SalesExecutive })
            if (!await roleManager.RoleExistsAsync(role)) await roleManager.CreateAsync(new IdentityRole(role));

        await CreateUser(userManager, "admin@acxiomcrm.local", "Admin", "System Administrator", "Admin@123");
        await CreateUser(userManager, "manager@acxiomcrm.local", "Manager", "CRM Manager", "Manager@123");
        await CreateUser(userManager, "sales@acxiomcrm.local", "SalesExecutive", "Sales Executive", "Sales@123");
    }

    private static async Task CreateUser(UserManager<ApplicationUser> manager, string email, string role, string name, string password)
    {
        var user = await manager.FindByEmailAsync(email);
        if (user != null) return;
        user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true, FullName = name, IsActive = true };
        var result = await manager.CreateAsync(user, password);
        if (result.Succeeded) await manager.AddToRoleAsync(user, role);
    }
}
