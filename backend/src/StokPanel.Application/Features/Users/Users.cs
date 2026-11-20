using StokPanel.Application.Abstraction;
using StokPanel.Application.Common;
using StokPanel.Application.Security;
using StokPanel.Domain.AuditLogs;
using StokPanel.Domain.Common;

namespace StokPanel.Application.Features.Users
{
    public sealed record SetRoleRequest(string Role);

    public sealed class GetUsersHandler(IIdentityService identityService)
    {
        public Task<List<UserDto>> HandleAsync() => identityService.GetUsersAsync();
    }

    public sealed class SetUserRoleHandler(IIdentityService identityService, ICurrentUser currentUser, IAppDbContext db)
    {
        public async Task HandleAsync(string userId, SetRoleRequest request)
        {
            if (!Roles.All.Contains(request.Role))
                throw new DomainException("Geçersiz rol");

            // Admin kendini düşüremez → sistemde her zaman en az bir admin kalır
            if (userId == currentUser.UserId)
                throw new DomainException("Kendi rolünüzü değiştiremezsiniz");

            var target = await identityService.GetUserAsync(userId)
                ?? throw new NotFoundException("Kullanıcı bulunamadı");

            await identityService.SetRoleAsync(userId, request.Role);

            // Rol değişikliği Identity'de yapılıyor, domain event üretmiyor → log elle ekleniyor
            db.AuditLogs.Add(AuditLog.Create(
                AuditEntityTypes.User, "Rol değişti", userId,
                $"{target.FullName} kullanıcısının rolü \"{request.Role}\" yapıldı",
                currentUser.UserId, currentUser.UserName));
            await db.SaveChangesAsync();
        }
    }
}
