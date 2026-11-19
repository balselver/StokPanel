using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StokPanel.Application.Abstraction;
using StokPanel.Infrastructure.Persistence;

namespace StokPanel.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastrucutre(this IServiceCollection services, IConfiguration configuration)
        {
            //ayar olmadan uygulama açılmaz
            var connectionString = configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException("ConncetionString: Default ayarı bulunamadı");

            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

            services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

            return services;
        }
    }
}
