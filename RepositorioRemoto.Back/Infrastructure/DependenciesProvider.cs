using Microsoft.Extensions.DependencyInjection;
using RepositorioRemoto.Back.Infraestructure.Interfaces;

namespace RepositorioRemoto.Back.Infrastructure;

public static class DependenciesProvider {
    /// <summary>
    /// Método estático que centraliza y simplifica la creación de las classes que implementan las interfaces <see cref="ITransientService"/> <see cref="IScopedService"/> ISingletonServicez<see cref="ISingletonService"/>
    /// con el rango de vida establecido por las mismas interfaces
    /// </summary>
    /// <returns>Un <see cref="IServiceProvider"/> con el contenedor de dependencias construido.</returns>
    public static IServiceProvider ServicesProvider() {
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
        return services.BuildServiceProvider();
    }
}