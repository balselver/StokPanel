using StokPanel.Application.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace StokPanel.Application.Features.Auth
{
    public sealed record LoginCommand(string Email, string Password, bool RememberMe);

    public sealed class LoginHandler(IIdentityService identityService)
    {
        public Task<UserDto?> LoginHandleAsync(LoginCommand command)
            => identityService.LoginAsync(command.Email?.Trim() ?? string.Empty, command.Password, command.RememberMe);
    }   

    public sealed class LogoutHandler(IIdentityService identityService)
    {
        public Task LogoutHandleAsync() => identityService.LogoutAsync();
    }

    public sealed class GetCurrentUserHandler(ICurrentUser currentUser, IIdentityService identityService)
    {
        public async Task<UserDto?> GetCurrHandleAsync()
            => currentUser.UserId is null ? null : await identityService.GetUserAsync(currentUser.UserId);
    }
    
}
