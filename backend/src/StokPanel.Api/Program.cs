using StokPanel.Infrastructure;
using StokPanel.Api.Endpoints;
using StokPanel.Api.Infrastructure;
using StokPanel.Application;
using StokPanel.Application.Security;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "StokPanel.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;

    // API'de login sayfasına yönlendirme yok, durum kodu dön
    options.Events.OnRedirectToLogin = ctx => { ctx.Response.StatusCode = 401; return Task.CompletedTask; };
    options.Events.OnRedirectToAccessDenied = ctx => { ctx.Response.StatusCode = 403; return Task.CompletedTask; };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(Policies.AdminOnly, policy => policy.RequireRole(Roles.Admin));

builder.Services.AddExceptionHandler<AppExceptionHandler>();
builder.Services.AddProblemDetails();

// Enum'lar JSON'da sayı değil metin olsun: "In" / "Out"
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await app.Services.InitializeDatabaseAsync();

app.UseExceptionHandler();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/health", () => Results.Ok(new { status = "ok", time = DateTime.UtcNow }));
app.MapAuthEndpoints();
app.MapCategoryEndpoints();
app.MapProductEndpoints();
app.MapDashboardEndpoints();
app.MapUserEndpoints();
app.MapAuditLogEndpoints();

// Bilinmeyen /api adresleri React sayfası değil, JSON 404 dönsün
app.Map("/api/{**rest}", () => Results.NotFound(new { error = "Böyle bir API ucu yok" }));

// Geri kalan her adres React'a gider (/urunler, /yonetim gibi React Router adresleri)
app.MapFallbackToFile("index.html");

app.Run();
