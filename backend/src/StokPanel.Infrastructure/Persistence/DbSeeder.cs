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

            var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var role in Roles.All)
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

            var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
            if ((await userManager.GetUsersInRoleAsync(Roles.Admin)).Count > 0)
                return;

            var config = sp.GetRequiredService<IConfiguration>();
            var email = config["Seed:AdminEmail"];
            var password = config["Seed:AdminPassword"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("İlk admin için Seed:AdminEmail ve Seed:AdminPassword ayarları gerekli");

            if (await userManager.FindByEmailAsync(email) is not null)
                throw new InvalidOperationException($"{email} normal bir kullanıcıya ait, admin yetkisi verilmedi. ");

            var admin = new ApplicationUser { UserName = email, Email = email, FullName = "Sistem Yöneticisi", EmailConfirmed = true };
            var result = await userManager.CreateAsync(admin, password);
            if (!result.Succeeded)
                throw new InvalidOperationException("Admin oluşturulamadı : " + string.Join(" ", result.Errors.Select(e => e.Description)));

            await userManager.AddToRoleAsync(admin, Roles.Admin);



        }
    }
}
