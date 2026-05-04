using MaisonBean.Domain.Entities;
using Microsoft.AspNetCore.Identity;

public static class DbSeeder
{
    public static async Task SeedAdminAsync(
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole<int>> roleManager)
    {
        const string adminRole = "Admin";
        const string adminEmail = "admin@gmail.com";
        const string adminPassword = "Admin@123";

        // 1. Create Role
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole<int>
            {
                Name = adminRole,
                NormalizedName = adminRole.ToUpper()
            });
        }

        // 2. Create or Get Admin User
        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin == null)
        {
            admin = new AppUser
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, adminPassword);

            if (!result.Succeeded)
            {
                throw new Exception("Admin creation failed: " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // 3. Ensure correct role assignment
        var roles = await userManager.GetRolesAsync(admin);

        if (!roles.Contains(adminRole))
        {
            // Remove wrong role if exists
            if (roles.Contains("admin"))
                await userManager.RemoveFromRoleAsync(admin, "admin");

            await userManager.AddToRoleAsync(admin, adminRole);
        }
    }
}