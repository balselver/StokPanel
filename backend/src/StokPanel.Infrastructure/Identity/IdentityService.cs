using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StokPanel.Application.Abstraction;
using StokPanel.Application.Security;
using StokPanel.Domain.Common;
using StokPanel.Infrastructure.Persistence;

namespace StokPanel.Infrastructure.Identity
{
    internal sealed class IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        AppDbContext db) : IIdentityService
    {
        public async Task<UserDto?> GetUserAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            return user is null ? null : await ToDoAsync(user);
        }

        public async Task<Dictionary<string, string>> GetUserNameAsync(IEnumerable<string> userIds)
        {
            var ids = userIds.Distinct().ToList();
            return await db.Users
                .Where(u => ids.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.FullName);
        }

        public async Task<List<UserDto>> GetUsersAsync()
        {
            var users = await db.Users.AsNoTracking().OrderBy(u => u.FullName).ToListAsync();

            var userRoles = await (
                from ur in db.UserRoles
                join r in db.Roles on ur.RoleId equals r.Id
                select new { ur.UserId, RoleName = r.Name! }
            ).ToListAsync();

            var rolesByUser = userRoles.ToLookup(x => x.UserId, x => x.RoleName);

            return users
                .Select(u => new UserDto(u.Id, u.FullName, u.Email ?? string.Empty, rolesByUser[u.Id].ToArray()))
                .ToList();
        }

        public async Task<UserDto?> LoginAsync(string email, string password, bool rememberMe)
        {
            var result = await signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: true);

            if (result.IsLockedOut)
                throw new DomainException("Çok fazla hatalı deneme. 10 dakika sonra tekrar deneyiniz.");

            if (!result.Succeeded)
                return null;

            var user = await userManager.FindByNameAsync(email);
            return user is null ? null : await ToDoAsync(user);
        }

        public Task LogoutAsync() => signInManager.SignOutAsync();
 

        public async Task<UserDto> RegisterAsync(string fullName, string email, string password)
        {
            var user = new ApplicationUser { UserName = email, Email = email, FullName = fullName };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new DomainException(string.Join(" ", result.Errors.Select(e =>e.Description)));

            await userManager.AddToRoleAsync(user, Roles.Personel);
            await signInManager.SignInAsync(user, isPersistent: false);

            return new UserDto(user.Id, user.FullName, email, [Roles.Personel]);
        }

        public async Task<bool> SetRoleAsync(string userId, string role)
        {

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return false;

            var currentRoles = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, currentRoles);
            await userManager.AddToRoleAsync(user, role);

            // Damga değişince kullanıcının cookie'si en geç 1 dk içinde yeni rolle yenilenir
            await userManager.UpdateSecurityStampAsync(user);
            return true;
        }

        private async Task<UserDto> ToDoAsync(ApplicationUser user)
        {
            var roles = await userManager.GetRolesAsync(user);
            return new UserDto(user.Id, user.FullName, user.Email ?? string.Empty, roles.ToArray());
        }


    }
}
