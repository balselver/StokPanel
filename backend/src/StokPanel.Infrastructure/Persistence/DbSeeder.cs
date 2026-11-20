using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StokPanel.Application.Security;
using StokPanel.Infrastructure.Identity;

namespace StokPanel.Infrastructure.Persistence
{
    internal static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var sp = scope.ServiceProvider;

            // 1) Eksik migration varsa uygula
            var db = sp.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();

            // 2) Roller
            var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var role in Roles.All)
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

            // 3) İlk admin
            var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
            var config = sp.GetRequiredService<IConfiguration>();
            var admin = await EnsureAdminAsync(userManager, config);

            // 4) Demo verisi (isteğe bağlı)
            if (config.GetValue<bool>("Seed:DemoData"))
            {
                var demoPassword = config["Seed:DemoPassword"]
                    ?? throw new InvalidOperationException("Seed:DemoData açık ama Seed:DemoPassword ayarı yok");
                var personelEmail = config["Seed:DemoPersonelEmail"] ?? "personel@stokpanel.local";

                await DemoDataSeeder.SeedAsync(db, userManager, admin.Id, personelEmail, demoPassword);
            }
        }

        private static async Task<ApplicationUser> EnsureAdminAsync(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            var existing = (await userManager.GetUsersInRoleAsync(Roles.Admin)).FirstOrDefault();
            if (existing is not null)
                return existing;

            var email = config["Seed:AdminEmail"];
            var password = config["Seed:AdminPassword"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("İlk admin için Seed:AdminEmail ve Seed:AdminPassword ayarları gerekli (User Secrets)");
            if (await userManager.FindByEmailAsync(email) is not null)
                throw new InvalidOperationException($"{email} normal bir kullanıcıya ait, admin yetkisi verilmedi");

            var admin = new ApplicationUser { UserName = email, Email = email, FullName = "Sistem Yöneticisi", EmailConfirmed = true };
            var result = await userManager.CreateAsync(admin, password);
            if (!result.Succeeded)
                throw new InvalidOperationException("Admin oluşturulamadı: " + string.Join(" ", result.Errors.Select(e => e.Description)));

            await userManager.AddToRoleAsync(admin, Roles.Admin);
            return admin;
        }
    }
}