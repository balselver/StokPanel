using StokPanel.Application.Features.Auth;

namespace StokPanel.Api.Endpoints
{
    public static class AuthEnpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/auth").WithTags("Auth");

            group.MapPost("/register", async (RegisterCommand command, RegisterHandler handler)
                =>Results.Ok(await handler.RegisterHandleAsync(command)));

            group.MapPost("/login", async (LoginCommand command, LoginHandler handler) =>
            {
                var user = await handler.LoginHandleAsync(command);
                return user is null
                    ? Results.Json(new { error = "E-posta veya şifre hatalı" }, statusCode: StatusCodes.Status401Unauthorized)
                    : Results.Ok(user);
            });

            group.MapPost("/logout", async (LogoutHandler handler) =>
            {
                await handler.LogoutHandleAsync();
                return Results.NoContent();
            }).RequireAuthorization();

            group.MapGet("/me", async (GetCurrentUserHandler handler) =>
            {
                var user = await handler.GetCurrHandleAsync();
                return user is null ? Results.Unauthorized() : Results.Ok(user);
            }).RequireAuthorization();
        }
    }
}
