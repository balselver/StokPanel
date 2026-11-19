using StokPanel.Application.Abstraction;
using StokPanel.Domain.Common;

namespace StokPanel.Application.Features.Auth
{
    public sealed record RegisterCommand(string FullName, string Email, string Password, string ConfirmPassword);
    public sealed class RegisterHandler(IIdentityService identityService)
    {
        public async Task<UserDto> RegisterHandleAsync(RegisterCommand command)
        {
            var fullName = command.FullName?.Trim() ?? string.Empty;

            if (fullName.Length == 0)
                throw new DomainException("Ad Soyad boş olamaz");

            if (command.Password != command.ConfirmPassword)
                throw new DomainException("Şifreler eşleşmiyor");

            return await identityService.RegisterAsync(fullName, command.Email?.Trim() ?? string.Empty, command.Password);                 
        }
    }   
}
