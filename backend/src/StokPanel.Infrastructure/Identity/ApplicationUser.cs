using Microsoft.AspNetCore.Identity;

namespace StokPanel.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}
