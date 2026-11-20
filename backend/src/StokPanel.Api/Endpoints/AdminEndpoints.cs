using StokPanel.Application.Features.AuditLogs;
using StokPanel.Application.Features.Dashboard;
using StokPanel.Application.Features.Users;
using StokPanel.Application.Security;

namespace StokPanel.Api.Endpoints
{
    public static class AdminEndpoints
    {
        public static void MapDashboardEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/dashboard", (GetDashboardHandler h, CancellationToken ct) => h.HandleAsync(ct))
                .WithTags("Gösterge")
                .RequireAuthorization();
        }

        public static void MapUserEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/users").WithTags("Kullanıcılar").RequireAuthorization(Policies.AdminOnly);

            group.MapGet("/", (GetUsersHandler h) => h.HandleAsync());

            group.MapPut("/{id}/role", async (string id, SetRoleRequest r, SetUserRoleHandler h) =>
            {
                await h.HandleAsync(id, r);
                return Results.NoContent();
            });
        }

        public static void MapAuditLogEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/audit-logs", ([AsParameters] AuditLogFilter filter, GetAuditLogsHandler h, CancellationToken ct)
                    => h.HandleAsync(filter, ct))
                .WithTags("İşlem Geçmişi")
                .RequireAuthorization(Policies.AdminOnly);
        }
    }
}
