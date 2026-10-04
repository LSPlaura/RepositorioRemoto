using Microsoft.Extensions.DependencyInjection;
using RepositorioRemoto.Back.Infraestructure.Interfaces;

namespace RepositorioRemoto.Back.Infraestructure;

public static class DependenciesProvider
{
    /// <summary>
    /// Método estático que centraliza y simplifica la creación de las classes que implementan las interfaces <see cref="ITransientService"/> <see cref="IScopedService"/> ISingletonServicez<see cref="ISingletonService"/>
    /// con el rango de vida establecido por las mismas interfaces
    /// </summary>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection ServicesProvider()
    {
        ServiceCollection services = new ServiceCollection();
        services.Scan(scan => scan
            .FromAssemblyOf<Program>()
            .AddClasses(classes => classes.AssignableTo<ITransientService>())
            .AsImplementedInterfaces()
            .WithTransientLifetime()
            .AddClasses(classes => classes.AssignableTo<IScopedService>())
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo<ISingletonService>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime()
        );
        return services;
    }
}