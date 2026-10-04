using Microsoft.Extensions.DependencyInjection;
using RepositorioRemoto.Back.Infraestructure;
using RepositorioRemoto.Back.Infrastructure;
using Serilog;
using Serilog.Events;

namespace RepositorioRemoto.Back;

class Program {
    static void Main(string[] args) {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console(
                outputTemplate:
                "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u3}] {Message:lj}{NewLine}{Exception}"
            )
            .CreateLogger();

        Log.Information("Aplicación iniciada");

        IServiceCollection services = DependenciesProvider.ServicesProvider()
            .AddDatabase()
            .AddCache();
        IServiceProvider provider = services.BuildServiceProvider();
    }
}    