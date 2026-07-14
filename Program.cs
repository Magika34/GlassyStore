using GlassyStore.Data;
using GlassyStore.Repositories;
using GlassyStore.Services;
using GlassyStore.Mapping;
using GlassyStore.Services.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GlassyStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession();

            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
            builder.Services.AddScoped<IEmailService, EmailService>();

            var app = builder.Build();

            // Seed default roles and an admin user
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                // Run seeding synchronously at startup for local/dev scenarios
                Data.SeedData.InitializeAsync(services).GetAwaiter().GetResult();

                // Ensure an administrator user exists so you can log in as admin@example.com
                // This runs at startup in development; remove or guard for production use.
                try
                {
                    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    var adminRole = "Administrator";
                    if (!roleManager.RoleExistsAsync(adminRole).GetAwaiter().GetResult())
                    {
                        roleManager.CreateAsync(new IdentityRole(adminRole)).GetAwaiter().GetResult();
                    }

                    var adminEmail = "admin@example.com";
                    var adminPassword = "Admin123!";

                    var admin = userManager.FindByEmailAsync(adminEmail).GetAwaiter().GetResult();
                    if (admin == null)
                    {
                        admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                        var createResult = userManager.CreateAsync(admin, adminPassword).GetAwaiter().GetResult();
                        if (createResult.Succeeded)
                        {
                            userManager.AddToRoleAsync(admin, adminRole).GetAwaiter().GetResult();
                        }
                        else
                        {
                            // If creation fails, write errors to console for debugging
                            foreach (var e in createResult.Errors)
                            {
                                Console.WriteLine($"Seed admin error: {e.Code} - {e.Description}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error ensuring admin user: {ex.Message}");
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
