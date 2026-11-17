using AppointMe.Api.Auth;
using AppointMe.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace AppointMe.Api.Data;

// Админ при запуске, без дубликатов
public static class AdminSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var email = config["Seed:AdminEmail"];
        var password = config["Seed:AdminPassword"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }
        if (await userManager.FindByEmailAsync(email) != null)
        {
            return;
        }

        var admin = new ApplicationUser
        {
            Email = email,
            UserName = email,
            FullName = "Admin"
        };

        var result = await userManager.CreateAsync(admin, password);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException("Не удалось создать админа: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        await userManager.AddToRoleAsync(admin, Roles.Admin);
    }
}