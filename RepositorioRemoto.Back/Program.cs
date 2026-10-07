using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using RepositorioRemoto.Back.Api;
using RepositorioRemoto.Back.Cache.Common;
using RepositorioRemoto.Back.Config;
using RepositorioRemoto.Back.Dto.Users;
using RepositorioRemoto.Back.Entity;
using RepositorioRemoto.Back.Enum;
using RepositorioRemoto.Back.Errors;
using RepositorioRemoto.Back.Infrastructure;
using RepositorioRemoto.Back.Models;
using RepositorioRemoto.Back.Models.Notification;
using RepositorioRemoto.Back.Notifications;
using RepositorioRemoto.Back.Repositories;
using RepositorioRemoto.Back.Services.Notifications;
using RepositorioRemoto.Back.Services.User;
using RepositorioRemoto.Back.Storage;
using System.Reactive.Linq;
using System.Text;
using RepositorioRemoto.Back.Mappers;
using RepositorioRemoto.Back.Services.Background;

namespace RepositorioRemoto.Back;

public class Program {
    private static int _correctas;
    private static int _fallidas;

    public static async Task Main(string[] args) {
        Console.OutputEncoding = Encoding.UTF8;

        // 1. Cargar configuración según el entorno
        Configuracion.Inicializar(args);

        // 2. Registro automático de servicios con Scrutor (ITransientService, IScopedService, ISingletonService)
        var services = DependenciesProvider.ServicesProvider();

        // 3. Módulos de infraestructura y APIs externas
        services.AddDatabase();
        services.AddCache();
        services.AddExternalApis();

        // 4. Construir el contenedor de dependencias
        using var provider = services.BuildServiceProvider();

        // 5. Inicializar el esquema de la base de datos
        provider.InitializeDatabase();

        // 6. Configuración de notificaciones y suscripción Rx.NET
        var notificationService = provider.GetRequiredService<INotificationService>();
        var notificaciones = new List<Notification>();

        using var subscription = notificationService.Observable.Subscribe(notification => {
            notificaciones.Add(notification);
            MostrarNotificacion(notification);
        });

        MostrarCabecera();

        // 7. Ejecución del banco de pruebas
        await EjecutarPruebaAsync("1. ARRANQUE - CARGA INICIAL", () => ProbarCargaInicialAsync(provider));
        await EjecutarPruebaAsync("2. GET ALL - BASE DE DATOS LOCAL", () => ProbarGetAllLocalAsync(provider));
        await EjecutarPruebaAsync("3. GET ALL - BD VACÍA → API", () => ProbarGetAllApiAsync(provider));
        await EjecutarPruebaAsync("4. GET BY ID - API → BD → CACHÉ", () => ProbarGetByIdApiAsync(provider));
        await EjecutarPruebaAsync("5. GET BY ID - BASE DE DATOS → CACHÉ", () => ProbarGetByIdBaseDatosAsync(provider));
        await EjecutarPruebaAsync("6. GET BY ID - CACHÉ", () => ProbarGetByIdCacheAsync(provider));
        await EjecutarPruebaAsync("7. GET BY ID - USUARIO INEXISTENTE", () => ProbarGetByIdInexistenteAsync(provider));
        await EjecutarPruebaAsync("8. CREATE - USUARIO VÁLIDO", () => ProbarCreateValidoAsync(provider, notificaciones));
        await EjecutarPruebaAsync("9. CREATE - CASOS INVÁLIDOS", () => ProbarCreateInvalidosAsync(provider, notificaciones));
        await EjecutarPruebaAsync("10. CREATE - NULL", () => ProbarCreateNullAsync(provider, notificaciones));
        await EjecutarPruebaAsync("11. UPDATE - USUARIO VÁLIDO", () => ProbarUpdateValidoAsync(provider, notificaciones));
        await EjecutarPruebaAsync("12. UPDATE - IDS DIFERENTES", () => ProbarUpdateIdsDiferentesAsync(provider, notificaciones));
        await EjecutarPruebaAsync("13. UPDATE - DATOS INVÁLIDOS", () => ProbarUpdateInvalidoAsync(provider, notificaciones));
        await EjecutarPruebaAsync("14. UPDATE - USUARIO INEXISTENTE", () => ProbarUpdateInexistenteAsync(provider, notificaciones));
        await EjecutarPruebaAsync("15. UPDATE - NULL", () => ProbarUpdateNullAsync(provider, notificaciones));
        await EjecutarPruebaAsync("16. DELETE - USUARIO VÁLIDO", () => ProbarDeleteValidoAsync(provider, notificaciones));
        await EjecutarPruebaAsync("17. DELETE - USUARIO INEXISTENTE", () => ProbarDeleteInexistenteAsync(provider, notificaciones));
        await EjecutarPruebaAsync("18. EXPORTAR USUARIOS A JSON", () => ProbarExportacionAsync(provider));
        await EjecutarPruebaAsync("19. NOTIFICACIONES RX.NET", () => ProbarNotificacionesAsync(notificaciones));
        await EjecutarPruebaAsync("20. BACKGROUND SERVICE - SINCRONIZACIÓN", () => ProbarBackgroundServiceAsync(provider));

        MostrarResumenFinal();
    }

    private static async Task ProbarCargaInicialAsync(IServiceProvider provider) {
        using var scope = provider.CreateScope();

        var api = scope.ServiceProvider.GetRequiredService<IApiJsonPlaceHolder>();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var cache = scope.ServiceProvider.GetRequiredService<ICache>();

        await cache.RemoveAllAsync();

        var deleteResult = await repository.DeleteAllAsync();

        Comprobar(deleteResult.IsSuccess, "Base de datos local limpiada");
        
        var users = await api.GetUserAsync();
        
        Comprobar(users.Count > 0, $"Se han cargado {users.Count} usuarios desde la API");

        foreach (var u in users)
        {
          var result = await repository.CreateAsync(u);
          Console.WriteLine($"{result.Value}");
        }

        var locales = (await repository.GetAllAsync()).ToList();

        Comprobar(locales.Count == users.Count, "Los usuarios se han guardado en la BD local");

        MostrarUsuarios(locales);
    }

    private static async Task ProbarGetAllLocalAsync(IServiceProvider provider) {
        await PrepararDatosAsync(provider);

        using var scope = provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<IUserService>();
        var usuarios = (await service.GetAllAsync()).ToList();

        MostrarUsuarios(usuarios);

        Comprobar(usuarios.Count > 0, "GetAll devuelve usuarios desde la BD local");
    }

    private static async Task ProbarGetAllApiAsync(IServiceProvider provider) {
        using var scope = provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<IUserService>();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var cache = scope.ServiceProvider.GetRequiredService<ICache>();

        await cache.RemoveAllAsync();

        var deleteResult = await repository.DeleteAllAsync();

        Comprobar(deleteResult.IsSuccess, "BD vaciada correctamente");

        var antes = (await repository.GetAllAsync()).ToList();

        Comprobar(antes.Count == 0, "La BD está vacía antes del GET ALL");

        var usuarios = (await service.GetAllAsync()).ToList();
        Comprobar(usuarios.Count > 0, "Los usuarios se obtienen desde la API");
        var listaAgregada = new List<User>();
        foreach (var user in usuarios)
        {
            var request = new CreateUserRequest(user.Name, user.UserName, user.Email, user.Address.ToDto(), user.Phone, user.Website, user.Company.ToDto() );
            var a = await service.CreateAsync(request);
            listaAgregada.Add(a.Value);
        }

        var despues = (await repository.GetAllAsync()).ToList();

        Comprobar(despues.Count == usuarios.Count, "Los usuarios de la API se almacenan en BD");
    }

    private static async Task ProbarGetByIdApiAsync(IServiceProvider provider) {
        using var scope = provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<IUserService>();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var cache = scope.ServiceProvider.GetRequiredService<ICache>();

        await cache.RemoveAllAsync();
        await repository.DeleteAllAsync();

        var result = await service.GetByIdAsync(1);

        MostrarResultado(result);

        Comprobar(result.IsSuccess, "Usuario obtenido desde la API");

        var local = await repository.GetByIdAsync(1);

        Comprobar(local.IsSuccess, "El usuario obtenido desde API se almacena en BD");

        var cacheado = await cache.GetAsync<User>("User:1");

        Comprobar(cacheado is not null, "El usuario obtenido desde API se almacena en caché");
    }

    private static async Task ProbarGetByIdBaseDatosAsync(IServiceProvider provider) {
        await PrepararDatosAsync(provider);

        using var scope = provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<IUserService>();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var cache = scope.ServiceProvider.GetRequiredService<ICache>();

        var local = await repository.GetByIdAsync(1);

        Comprobar(local.IsSuccess, "El usuario existe en la BD");

        await cache.RemoveAsync("User:1");

        var cacheAntes = await cache.GetAsync<User>("User:1");

        Comprobar(cacheAntes is null, "El usuario no está inicialmente en caché");

        var result = await service.GetByIdAsync(1);

        MostrarResultado(result);

        var cacheDespues = await cache.GetAsync<User>("User:1");

        Comprobar(result.IsSuccess, "Usuario obtenido correctamente desde BD");
        Comprobar(cacheDespues is not null, "El usuario obtenido desde BD se añade a caché");
    }

    private static async Task ProbarGetByIdCacheAsync(IServiceProvider provider) {
        await PrepararDatosAsync(provider);

        using var scope = provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<IUserService>();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var cache = scope.ServiceProvider.GetRequiredService<ICache>();

        var usuario = CrearUsuarioValido().ToModel();
        var usuarioCache = usuario with
        {
            Id = 1
        };

        await cache.SetAsync("User:1", usuarioCache);

        var result = await service.GetByIdAsync(1);

        MostrarResultado(result);

        Comprobar(result.IsSuccess, "GET realizado correctamente");
        Comprobar(result.IsSuccess && result.Value.Name == "Usuario Desde Cache", "La caché tiene prioridad sobre BD y API");
    }

    private static async Task ProbarGetByIdInexistenteAsync(IServiceProvider provider) {
        await PrepararDatosAsync(provider);

        using var scope = provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<IUserService>();
        var cache = scope.ServiceProvider.GetRequiredService<ICache>();

        await cache.RemoveAsync("User:9999");

        var result = await service.GetByIdAsync(9999);

        MostrarResultado(result);

        Comprobar(result.IsFailure, "El usuario inexistente devuelve Failure");
    }

    private static async Task ProbarCreateValidoAsync(IServiceProvider provider, List<Notification> notificaciones) {
        await PrepararDatosAsync(provider);

        using var scope = provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<IUserService>();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        var numeroNotificaciones = notificaciones.Count;
        var request = CrearUsuarioValido();
        var result = await service.CreateAsync(request);

        MostrarResultado(result);

        Comprobar(result.IsSuccess, "El usuario válido se crea correctamente");

        if (result.IsFailure) return;

        var local = await repository.GetByIdAsync(result.Value.Id);

        Comprobar(local.IsSuccess, "El usuario creado existe en BD");

        var nuevas = notificaciones.Skip(numeroNotificaciones).ToList();
        var notificacion = nuevas.FirstOrDefault(n => n.Tipo == TypeNotification.Create);

        Comprobar(notificacion is not null, "Create genera una notificación de tipo Create");

        if (notificacion is not null) {
            Comprobar(notificacion.Mensaje == $"Se ha creado el usuario con ID {result.Value.Id}.", "El mensaje de Create es correcto");
            Comprobar(notificacion.Timestamp != default, "La notificación Create contiene fecha");
        }
    }

    private static async Task ProbarCreateInvalidosAsync(IServiceProvider provider, List<Notification> notificaciones) {
        using var scope = provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<IUserService>();
        var valido = CrearUsuarioValido();
        var numeroNotificaciones = notificaciones.Count;

        await ComprobarCreateFailure(service, valido with { Name = "" }, "Nombre vacío");
        await ComprobarCreateFailure(service, valido with { Name = "A" }, "Nombre demasiado corto");
        await ComprobarCreateFailure(service, valido with { Name = "123456" }, "Nombre inválido");
        await ComprobarCreateFailure(service, valido with { UserName = "" }, "Username vacío");
        await ComprobarCreateFailure(service, valido with { UserName = "a" }, "Username demasiado corto");
        await ComprobarCreateFailure(service, valido with { Email = "" }, "Email vacío");
        await ComprobarCreateFailure(service, valido with { Email = "correo-invalido" }, "Email inválido");
        await ComprobarCreateFailure(service, valido with { Phone = "" }, "Teléfono vacío");
        await ComprobarCreateFailure(service, valido with { Phone = "telefono" }, "Teléfono inválido");
        await ComprobarCreateFailure(service, valido with { Website = "" }, "Website vacío");
        await ComprobarCreateFailure(service, valido with { Website = "web" }, "Website inválida");

        await ComprobarCreateFailure(service, valido with {
            Address = new AddressDto("", "Apt 556", "Madrid", "28911", new GeoDto("-37.3159", "81.1496"))
        }, "Street vacío");

        await ComprobarCreateFailure(service, valido with {
            Address = new AddressDto("Kulas Light", "", "Madrid", "28911", new GeoDto("-37.3159", "81.1496"))
        }, "Suite vacío");

        await ComprobarCreateFailure(service, valido with {
            Address = new AddressDto("Kulas Light", "Apt 556", "", "28911", new GeoDto("-37.3159", "81.1496"))
        }, "City vacío");

        await ComprobarCreateFailure(service, valido with {
            Address = new AddressDto("Kulas Light", "Apt 556", "Madrid", "", new GeoDto("-37.3159", "81.1496"))
        }, "ZipCode vacío");

        await ComprobarCreateFailure(service, valido with {
            Address = new AddressDto("Kulas Light", "Apt 556", "Madrid", "28911", new GeoDto("latitud", "81.1496"))
        }, "Latitud inválida");

        await ComprobarCreateFailure(service, valido with {
            Address = new AddressDto("Kulas Light", "Apt 556", "Madrid", "28911", new GeoDto("-37.3159", "longitud"))
        }, "Longitud inválida");

        await ComprobarCreateFailure(service, valido with {
            Company = new CompanyDto("", "Software company", "Development services")
        }, "Nombre de empresa vacío");

        await ComprobarCreateFailure(service, valido with {
            Company = new CompanyDto("Test Company", "", "Development services")
        }, "CatchPhrase vacío");

        await ComprobarCreateFailure(service, valido with {
            Company = new CompanyDto("Test Company", "Software company", "")
        }, "Bs vacío");

        Comprobar(notificaciones.Count == numeroNotificaciones, "Los Create inválidos no generan notificaciones");
    }

    private static async Task ProbarCreateNullAsync(IServiceProvider provider, List<Notification> notificaciones) {
        using var scope = provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<IUserService>();
        var numeroNotificaciones = notificaciones.Count;

        var result = await service.CreateAsync(null!);

        MostrarResultado(result);

        Comprobar(result.IsFailure, "Create con null devuelve Failure");
        Comprobar(notificaciones.Count == numeroNotificaciones, "Create con null no genera notificación");
    }

    private static async Task ProbarUpdateValidoAsync(IServiceProvider provider, List<Notification> notificaciones) {
        await PrepararDatosAsync(provider);

        using var scope = provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<IUserService>();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var cache = scope.ServiceProvider.GetRequiredService<ICache>();

        await service.GetByIdAsync(1);

        var numeroNotificaciones = notificaciones.Count;
        var request = CrearUpdateValido(1);
        var result = await service.UpdateAsync(1, request);

        MostrarResultado(result);

        Comprobar(result.IsSuccess, "Usuario actualizado correctamente");

        if (result.IsFailure) return;

        var local = await repository.GetByIdAsync(1);

        Comprobar(local.IsSuccess && local.Value.Name == request.Name, "La BD contiene los datos actualizados");

        var cacheado = await cache.GetAsync<User>("User:1");

        Comprobar(cacheado is not null && cacheado.Name == request.Name, "La caché contiene los datos actualizados");

        var nuevas = notificaciones.Skip(numeroNotificaciones).ToList();
        var notificacion = nuevas.FirstOrDefault(n => n.Tipo == TypeNotification.Update);

        Comprobar(notificacion is not null, "Update genera una notificación de tipo Update");

        if (notificacion is not null) {
            Comprobar(notificacion.Mensaje == "Se ha actualizado el usuario con ID 1.", "El mensaje de Update es correcto");
            Comprobar(notificacion.Timestamp != default, "La notificación Update contiene fecha");
        }
    }

    private static async Task ProbarUpdateIdsDiferentesAsync(IServiceProvider provider, List<Notification> notificaciones) {
        await PrepararDatosAsync(provider);

        using var scope = provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<IUserService>();
        var numeroNotificaciones = notificaciones.Count;

        var request = CrearUpdateValido(2);
        var result = await service.UpdateAsync(1, request);

        MostrarResultado(result);

        Comprobar(result.IsFailure, "IDs diferentes devuelven Failure");
        Comprobar(notificaciones.Count == numeroNotificaciones, "Update con IDs diferentes no genera notificación");
    }

    private static async Task ProbarUpdateInvalidoAsync(IServiceProvider provider, List<Notification> notificaciones) {
        await PrepararDatosAsync(provider);

        using var scope = provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<IUserService>();
        var numeroNotificaciones = notificaciones.Count;

        var request = CrearUpdateValido(1) with { Email = "correo-invalido" };
        var result = await service.UpdateAsync(1, request);

        MostrarResultado(result);

        Comprobar(result.IsFailure, "Update con datos inválidos devuelve Failure");
        Comprobar(notificaciones.Count == numeroNotificaciones, "Update inválido no genera notificación");
    }

    private static async Task ProbarUpdateInexistenteAsync(IServiceProvider provider, List<Notification> notificaciones) {
        await PrepararDatosAsync(provider);

        using var scope = provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<IUserService>();
        var numeroNotificaciones = notificaciones.Count;

        var request = CrearUpdateValido(9999);
        var result = await service.UpdateAsync(9999, request);

        MostrarResultado(result);

        Comprobar(result.IsFailure, "No se puede actualizar un usuario inexistente");
        Comprobar(notificaciones.Count == numeroNotificaciones, "Update de usuario inexistente no genera notificación");
    }

    private static async Task ProbarUpdateNullAsync(IServiceProvider provider, List<Notification> notificaciones) {
        using var scope = provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<IUserService>();
        var numeroNotificaciones = notificaciones.Count;

        var result = await service.UpdateAsync(1, null!);

        MostrarResultado(result);

        Comprobar(result.IsFailure, "Update con null devuelve Failure");
        Comprobar(notificaciones.Count == numeroNotificaciones, "Update con null no genera notificación");
    }

    private static async Task ProbarDeleteValidoAsync(IServiceProvider provider, List<Notification> notificaciones) {
        await PrepararDatosAsync(provider);

        using var scope = provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<IUserService>();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var cache = scope.ServiceProvider.GetRequiredService<ICache>();

        var usuario = await service.GetByIdAsync(2);

        Comprobar(usuario.IsSuccess, "El usuario existe antes del Delete");

        var cacheAntes = await cache.GetAsync<User>("User:2");

        Comprobar(cacheAntes is not null, "El usuario está en caché antes del Delete");

        var numeroNotificaciones = notificaciones.Count;
        var result = await service.DeleteAsync(2);

        MostrarResultado(result);

        Comprobar(result.IsSuccess, "Usuario eliminado correctamente");

        if (result.IsFailure) return;

        var local = await repository.GetByIdAsync(2);

        Comprobar(local.IsFailure, "El usuario se elimina de la BD");

        var cacheDespues = await cache.GetAsync<User>("User:2");

        Comprobar(cacheDespues is null, "El usuario se elimina de caché");

        var nuevas = notificaciones.Skip(numeroNotificaciones).ToList();
        var notificacion = nuevas.FirstOrDefault(n => n.Tipo == TypeNotification.Delete);

        Comprobar(notificacion is not null, "Delete genera una notificación de tipo Delete");

        if (notificacion is not null) {
            Comprobar(notificacion.Mensaje == "Se ha eliminado el usuario con ID 2.", "El mensaje de Delete es correcto");
            Comprobar(notificacion.Timestamp != default, "La notificación Delete contiene fecha");
        }
    }

    private static async Task ProbarDeleteInexistenteAsync(IServiceProvider provider, List<Notification> notificaciones) {
        await PrepararDatosAsync(provider);

        using var scope = provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<IUserService>();
        var numeroNotificaciones = notificaciones.Count;

        var result = await service.DeleteAsync(9999);

        MostrarResultado(result);

        Comprobar(result.IsFailure, "Delete de usuario inexistente devuelve Failure");
        Comprobar(notificaciones.Count == numeroNotificaciones, "Delete inexistente no genera notificación");
    }

    private static async Task ProbarExportacionAsync(IServiceProvider provider) {
        await PrepararDatosAsync(provider);

        using var scope = provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<IUserService>();
        var result = await service.ExportToJsonAsync();

        if (result.IsSuccess) {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("RESULTADO: SUCCESS");
        } else {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("RESULTADO: FAILURE");
            Console.WriteLine($"Error: {result.Error.Message}");
        }

        Console.ResetColor();

        Comprobar(result.IsSuccess, "Exportación JSON realizada");

        if (result.IsFailure) return;

        var existe = File.Exists(Configuracion.UsersJsonPath);

        Comprobar(existe, "El fichero JSON existe físicamente");

        if (existe) {
            var info = new FileInfo(Configuracion.UsersJsonPath);

            Comprobar(info.Length > 0, "El fichero JSON contiene datos");

            Console.WriteLine($"Ruta: {info.FullName}");
            Console.WriteLine($"Tamaño: {info.Length} bytes");
        }
    }

    private static Task ProbarNotificacionesAsync(List<Notification> notificaciones) {
        Console.WriteLine($"Total de notificaciones recibidas: {notificaciones.Count}");
        Console.WriteLine();

        foreach (var notification in notificaciones) {
            Console.WriteLine($"[{notification.Timestamp:HH:mm:ss}] {notification.Tipo} -> {notification.Mensaje}");
        }

        var create = notificaciones.Count(n => n.Tipo == TypeNotification.Create);
        var update = notificaciones.Count(n => n.Tipo == TypeNotification.Update);
        var delete = notificaciones.Count(n => n.Tipo == TypeNotification.Delete);

        Console.WriteLine();
        Console.WriteLine($"Create: {create}");
        Console.WriteLine($"Update: {update}");
        Console.WriteLine($"Delete: {delete}");

        Comprobar(create > 0, "Se ha recibido al menos una notificación Create");
        Comprobar(update > 0, "Se ha recibido al menos una notificación Update");
        Comprobar(delete > 0, "Se ha recibido al menos una notificación Delete");
        Comprobar(notificaciones.Count > 0 && notificaciones.All(n => n.Timestamp != default), "Todas las notificaciones contienen Timestamp");
        Comprobar(notificaciones.Count > 0 && notificaciones.All(n => !string.IsNullOrWhiteSpace(n.Mensaje)), "Todas las notificaciones contienen Mensaje");

        return Task.CompletedTask;
    }
    
    private static async Task ProbarBackgroundServiceAsync(IServiceProvider provider) {
        await PrepararDatosAsync(provider);
    
        int idTemporal;
        string emailTemporal;
    
        using (var scope = provider.CreateScope()) {
            var service = scope.ServiceProvider.GetRequiredService<IUserService>();
            var cacheInicial = scope.ServiceProvider.GetRequiredService<ICache>();
    
            var request = CrearUsuarioValido() with {
                Name = "Usuario Background",
                UserName = "background.test",
                Email = "background.test@gmail.com"
            };
    
            var createResult = await service.CreateAsync(request);
    
            if (createResult.IsFailure) {
                Comprobar(false, $"No se ha podido crear el usuario temporal: {createResult.Error.Message}");
                return;
            }
    
            var usuarioTemporal = createResult.Value;
    
            idTemporal = usuarioTemporal.Id;
            emailTemporal = usuarioTemporal.Email;
    
            Comprobar(true, $"Usuario temporal creado con ID {idTemporal}");
    
            await cacheInicial.SetAsync($"User:{idTemporal}", usuarioTemporal);
    
            var cacheAntes = await cacheInicial.GetAsync<User>($"User:{idTemporal}");
    
            Comprobar(cacheAntes is not null, "Usuario temporal creado en caché");
        }
    
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine();
        Console.WriteLine("Iniciando BackgroundService...");
        Console.WriteLine("Esperando al ciclo real de sincronización de 60 segundos...");
        Console.ResetColor();
    
        var backgroundService = provider.GetRequiredService<IBackgroundService>();
    
        using var cancellationTokenSource = new CancellationTokenSource();
    
        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(65));
    
        try {
            await backgroundService.StartAsync(cancellationTokenSource.Token);
        } catch (OperationCanceledException) {
        }
    
        using var scopeVerificacion = provider.CreateScope();
    
        var repositoryVerificacion = scopeVerificacion.ServiceProvider.GetRequiredService<IUserRepository>();
        var cacheVerificacion = scopeVerificacion.ServiceProvider.GetRequiredService<ICache>();
    
        var usuarios = (await repositoryVerificacion.GetAllAsync()).ToList();
        var cacheDespues = await cacheVerificacion.GetAsync<User>($"User:{idTemporal}");
    
        var usuarioTemporalSigueExistiendo = usuarios.Any(u => u.Email == emailTemporal);
    
        Comprobar(
            !usuarioTemporalSigueExistiendo,
            "El BackgroundService elimina los datos locales anteriores"
        );
    
        Comprobar(
            cacheDespues is null,
            "El BackgroundService limpia la caché"
        );
    
        Comprobar(
            usuarios.Count > 0,
            "El BackgroundService vuelve a cargar usuarios desde la API"
        );
    
        Console.WriteLine($"Usuarios después de sincronizar: {usuarios.Count}");
    }

    private static async Task PrepararDatosAsync(IServiceProvider provider) {
        using var scope = provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<IUserService>();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var cache = scope.ServiceProvider.GetRequiredService<ICache>();

        await cache.RemoveAllAsync();
        await repository.DeleteAllAsync();
        await service.GetAllAsync();
    }

    private static CreateUserRequest CrearUsuarioValido() {
        return new CreateUserRequest(
            "Lucia Test",
            "lucia.test",
            "lucia.test@gmail.com",
            new AddressDto("Kulas Light", "Apt 556", "Madrid", "28911", new GeoDto("-37.3159", "81.1496")),
            "600123123",
            "luciatest.com",
            new CompanyDto("Test Company", "Software company", "Development services")
        );
    }

    private static UpdateUserRequest CrearUpdateValido(int id) {
        return new UpdateUserRequest(
            id,
            "Usuario Actualizado",
            "usuario.actualizado",
            "actualizado@gmail.com",
            new AddressDto("Gran Via 25", "Piso 2", "Madrid", "28013", new GeoDto("40.4168", "-3.7038")),
            "611222333",
            "usuarioactualizado.com",
            new CompanyDto("Updated Company", "Updated software", "Technology services")
        );
    }

    private static async Task ComprobarCreateFailure(IUserService service, CreateUserRequest request, string descripcion) {
        var result = await service.CreateAsync(request);

        Comprobar(result.IsFailure, descripcion);

        if (result.IsFailure) {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"   Error esperado: {result.Error.Message}");
            Console.ResetColor();
        }
    }

    private static void MostrarResultado(Result<User, DomainError> result) {
        if (result.IsSuccess) {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("RESULTADO: SUCCESS");
            Console.ResetColor();

            MostrarUsuario(result.Value);
        } else {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("RESULTADO: FAILURE");
            Console.WriteLine($"Error: {result.Error.Message}");
            Console.ResetColor();
        }
    }

    private static void MostrarUsuario(User usuario) {
        Console.WriteLine($"Id:       {usuario.Id}");
        Console.WriteLine($"Nombre:   {usuario.Name}");
        Console.WriteLine($"Username: {usuario.UserName}");
        Console.WriteLine($"Email:    {usuario.Email}");
        Console.WriteLine($"Teléfono: {usuario.Phone}");
        Console.WriteLine($"Website:  {usuario.Website}");
    }

    private static void MostrarUsuarios(IEnumerable<User> usuarios) {
        var lista = usuarios.ToList();

        Console.WriteLine();
        Console.WriteLine($"Usuarios encontrados: {lista.Count}");

        foreach (var usuario in lista) {
            Console.WriteLine($"[{usuario.Id}] {usuario.Name} | {usuario.UserName} | {usuario.Email}");
        }
    }

    private static void MostrarNotificacion(Notification notification) {
        Console.ForegroundColor = ConsoleColor.Magenta;

        Console.WriteLine();
        Console.WriteLine("╔════════════════ NOTIFICACIÓN ════════════════╗");
        Console.WriteLine($"  Tipo:    {notification.Tipo}");
        Console.WriteLine($"  Mensaje: {notification.Mensaje}");
        Console.WriteLine($"  Fecha:   {notification.Timestamp:dd/MM/yyyy HH:mm:ss}");
        Console.WriteLine("╚═══════════════════════════════════════════════╝");

        Console.ResetColor();
    }

    private static async Task EjecutarPruebaAsync(string titulo, Func<Task> prueba) {
        Separador(titulo);

        try {
            await prueba();
        } catch (Exception ex) {
            _fallidas++;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("✘ EXCEPCIÓN NO CONTROLADA");
            Console.WriteLine($"Tipo: {ex.GetType().Name}");
            Console.WriteLine($"Mensaje: {ex.Message}");
            Console.ResetColor();
        }
    }

    private static void Comprobar(bool condicion, string mensaje) {
        if (condicion) {
            _correctas++;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("✔ ");
        } else {
            _fallidas++;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("✘ ");
        }

        Console.WriteLine(mensaje);
        Console.ResetColor();
    }

    private static void Separador(string titulo) {
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("════════════════════════════════════════════════════════════");
        Console.WriteLine(titulo);
        Console.WriteLine("════════════════════════════════════════════════════════════");
        Console.ResetColor();
    }

    private static void MostrarCabecera() {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                 REPOSITORIO REMOTO                      ║");
        Console.WriteLine("║          PRÁCTICA 6 - PROGRAMA DE PRUEBAS              ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
        Console.ResetColor();
    }

    private static void MostrarResumenFinal() {
        var total = _correctas + _fallidas;

        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    RESULTADO FINAL                       ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
        Console.ResetColor();

        Console.WriteLine($"Comprobaciones realizadas: {total}");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Correctas: {_correctas}");
        Console.ResetColor();

        Console.ForegroundColor = _fallidas == 0 ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine($"Fallidas: {_fallidas}");
        Console.ResetColor();

        Console.WriteLine();

        if (_fallidas == 0) {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✔ TODAS LAS COMPROBACIONES HAN PASADO");
        } else {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("⚠ REVISA LAS COMPROBACIONES MARCADAS CON ✘");
        }

        Console.ResetColor();
    }
}