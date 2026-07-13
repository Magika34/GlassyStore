using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace GlassyStore.Data
{
    public static class SeedData
    {
        // Seeds roles and a default admin only when allowed (development or explicit config).
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var env = serviceProvider.GetRequiredService<IHostEnvironment>();
            var config = serviceProvider.GetRequiredService<IConfiguration>();

            // Allow seeding when running in Development or when explicitly enabled via configuration
            var allowSeed = env.IsDevelopment() || config.GetValue<bool>("Seed:Enable", false);
            if (!allowSeed)
            {
                Debug.WriteLine("SeedData: seeding disabled (not Development and Seed:Enable not true).");
                return;
            }

            string[] roles = new[] { "Administrator", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Admin account settings can be provided via configuration for non-dev scenarios
            var adminEmail = config["Seed:AdminEmail"] ?? "admin@localhost";
            var adminPassword = config["Seed:AdminPassword"];

            // In Development, allow the default weak password only for convenience
            if (string.IsNullOrEmpty(adminPassword) && env.IsDevelopment())
            {
                adminPassword = "Admin123!";
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                if (string.IsNullOrEmpty(adminPassword))
                {
                    Debug.WriteLine("SeedData: admin password not provided; skipping admin user creation.");
                    return;
                }

                adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrator");
                    Debug.WriteLine($"SeedData: created admin user {adminEmail}.");
                }
                else
                {
                    Debug.WriteLine("SeedData: failed to create admin user:");
                    foreach (var e in result.Errors)
                    {
                        Debug.WriteLine(e.Description);
                    }
                }
            }
        }
    }
}
