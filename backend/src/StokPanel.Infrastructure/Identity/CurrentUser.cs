using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using StokPanel.Application.Abstraction;

namespace StokPanel.Infrastructure.Identity
{
    internal sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
    {
        public string? UserId => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        public string? UserName => accessor.HttpContext?.User.Identity?.Name;
    }
}
