using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CityCard_API.Models.DB.Seeders;
public static class UserSeeder
{
    public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<CCUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var dbContext = serviceProvider.GetRequiredService<CityCardDBContext>();

        dbContext.Database.Migrate();

        // Check if the admin role exists; if not, create it
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // Check if the admin user exists; if not, create it
        var adminUser = await userManager.FindByEmailAsync("admin@citycard.ua");
        if (adminUser == null)
        {
            adminUser = new CCUser
            {
                UserName = "admin@citycard.ua",
                Email = "admin@citycard.ua",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, "admin");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
