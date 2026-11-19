using Microsoft.Extensions.DependencyInjection;

namespace StokPanel.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var handlerTypes = typeof(DependencyInjection).Assembly.GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false } && t.Name.EndsWith("Handler"));

            foreach (var type in handlerTypes)
                services.AddScoped(type);

            return services;
                
        }
    }
}
