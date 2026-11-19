using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace StokPanel.Application.Abstraction
{
    public sealed record UserDto(string Id, string FullName, string Email, string[] Roles);
    public interface IIdentityService
    {
        Task<UserDto> RegisterAsync(string fullName, string email, string password);
        Task<UserDto?> LoginAsync(string email, string password, bool rememberMe);
        Task LogoutAsync();
        Task<UserDto?> GetUserAsync(string userId);

        Task<List<UserDto>> GetUsersAsync();
        Task<bool> SetRoleAsync(string userId, string role);
        Task<Dictionary<string, string>> GetUserNameAsync(IEnumerable<string> userIds);
    }
}
