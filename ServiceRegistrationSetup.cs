using Microsoft.Extensions.DependencyInjection;

namespace AutoServiceRegistration.AspNetCore;

public static class ServiceRegistrationSetup
{
    public static IServiceCollection AddRegisterServices(this IServiceCollection services) =>
        services
            .AddServices(typeof(ISingletonService), ServiceLifetime.Singleton)
            .AddServices(typeof(IScopedService), ServiceLifetime.Scoped)
            .AddServices(typeof(ITransientService), ServiceLifetime.Transient);

    private static IServiceCollection AddServices(this IServiceCollection services, Type interfaceType,
        ServiceLifetime lifetime)
    {
        var interfaceTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(type => interfaceType.IsAssignableFrom(type)
                           && type.IsClass
                           && !type.IsAbstract)
            .SelectMany(type => type.GetInterfaces()
                .Where(interfaceType.IsAssignableFrom)
                .Select(service => new
                {
                    Service = service,
                    Implementation = type
                })
            );

        foreach (var type in interfaceTypes)
        {
            services.AddService(type.Service!, type.Implementation, lifetime);
        }

        return services;
    }

    private static IServiceCollection AddService(this IServiceCollection services, Type serviceType,
        Type implementationType, ServiceLifetime lifetime) =>
        lifetime switch
        {
            ServiceLifetime.Singleton => services.AddSingleton(serviceType, implementationType),
            ServiceLifetime.Scoped => services.AddScoped(serviceType, implementationType),
            ServiceLifetime.Transient => services.AddTransient(serviceType, implementationType),
            _ => throw new ArgumentException("Invalid lifeTime", nameof(lifetime))
        };
}