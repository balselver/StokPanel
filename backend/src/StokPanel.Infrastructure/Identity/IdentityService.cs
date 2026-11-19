using Microsoft.AspNetCore.Identity;
using StokPanel.Application.Abstraction;
using StokPanel.Application.Security;
using StokPanel.Domain.Common;

namespace StokPanel.Infrastructure.Identity
{
    internal sealed class IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager) : IIdentityService
    {
        public async Task<UserDto> GetUserAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            return user is null ? null : await ToDoAsync(user);
        }

        public async Task<UserDto> LoginAsync(string email, string password, bool rememberMe)
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

        private async Task<UserDto> ToDoAsync(ApplicationUser user)
        {
            var roles = await userManager.GetRolesAsync(user);
            return new UserDto(user.Id, user.FullName, user.Email ?? string.Empty, roles.ToArray());
        }
    }
}
