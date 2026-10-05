using CSharpFunctionalExtensions;
using Refit;
using RepositorioRemoto.Back.Api;
using RepositorioRemoto.Back.Cache.Common;
using RepositorioRemoto.Back.Config;
using RepositorioRemoto.Back.Dto.Users.Request;
using RepositorioRemoto.Back.Enum;
using RepositorioRemoto.Back.Errors;
using RepositorioRemoto.Back.Errors.Service;
using RepositorioRemoto.Back.Errors.Users;
using RepositorioRemoto.Back.Infrastructure.Interfaces;
using RepositorioRemoto.Back.Mappers;
using RepositorioRemoto.Back.Models.Notification;
using RepositorioRemoto.Back.Notifications;
using RepositorioRemoto.Back.Repositories;
using RepositorioRemoto.Back.Storage;
using RepositorioRemoto.Back.Validator;

namespace RepositorioRemoto.Back.Services.User;

/// <summary>
/// Gestiona usuarios mediante una API remota, un repositorio local y una caché.
/// Trabaja directamente con el modelo de dominio User.
/// </summary>
public class UserService(
    IValidate<Models.User> validator,
    IUserRepository repository,
    ICache cache,
    IUserStorage storage,
    INotificationService notificationService,
    IApiJsonPlaceHolder api
    ) : IUserService, IScopedService {
    
    /// <inheritdoc cref="IUserService.GetAllAsync"/>
    public async Task<IEnumerable<Models.User>> GetAllAsync() {
        var locales =  await repository.GetAllAsync();
        if (locales.Any()) return locales;

        var remotos = await api.GetUserAsync();
        foreach (var u in remotos) {
            await repository.CreateAsync(u);
        }
        return remotos;
    }

    /// <inheritdoc cref="IUserService.GetByIdAsync"/>
    public async Task<Result<Models.User, DomainError>> GetByIdAsync(int id) {
        try {
            var cacheado = await cache.GetAsync<Models.User>(GetKeyUser(id));
            if (cacheado is not null) return Result.Success<Models.User, DomainError>(cacheado);

            var local = await repository.GetByIdAsync(id);
            if (local.IsSuccess) {
                await cache.SetAsync(GetKeyUser(id), local.Value);
                return Result.Success<Models.User, DomainError>(local.Value);
            }

            var remoto = await api.GetUserByIdAsync(id);
            if (remoto is null) return Result.Failure<Models.User, DomainError>(UsersErrors.NotFoundError(id));

            var guardado = await repository.CreateAsync(remoto);
            if (guardado.IsFailure) return Result.Failure<Models.User, DomainError>(guardado.Error);

            await cache.SetAsync(GetKeyUser(id), guardado.Value);
            return Result.Success<Models.User, DomainError>(guardado.Value);
        } catch (ApiException ex) when (
            ex.StatusCode == System.Net.HttpStatusCode.NotFound) {
            return Result.Failure<Models.User, DomainError>(
                UsersErrors.NotFoundError(id));
        } catch (Exception) {
            return Result.Failure<Models.User, DomainError>(
                ServiceErrors.GetByIdError(id));
        }
    }

    /// <inheritdoc cref="IUserService.CreateAsync"/>
    public async Task<Result<Models.User, DomainError>> CreateAsync(CreateUserRequest request) {
        try {
            var usuario = request.ToModel();

            return await validator.Validate(usuario)
                .Map(_ => api.CreateUserAsync(request))
                .Map(creado => usuario with { Id = creado.Id })
                .Bind(u => repository.CreateAsync(u))
                .Tap(u => notificationService.Notificar(new Notification(
                    TypeNotification.Create,
                    $"Se ha creado el usuario con ID {u.Id}.",
                    DateTime.UtcNow)));
        } catch (Exception) {
            return Result.Failure<Models.User, DomainError>(ServiceErrors.CreateError());
        }
    }

    /// <inheritdoc cref="IUserService.UpdateAsync"/>
    public async Task<Result<Models.User, DomainError>> UpdateAsync(int id, UpdateUserRequest request) {
        try {
            if (id != request.Id) return Result.Failure<Models.User, DomainError>(ServiceErrors.UpdateError(id));
            if (validator.Validate(request.ToModel()).IsFailure) return Result.Failure<Models.User, DomainError>(ServiceErrors.UpdateError(id));

            return await ComprobarExistenciaAsync(id)
                .Tap(u => api.UpdateUserAsync(id, request))
                .Bind(u => repository.UpdateAsync(id, request.ToModel()))
                .Tap(u => notificationService.Notificar(new Notification(
                    TypeNotification.Update,
                    $"Se ha actualizado el usuario con ID {id}.",
                    DateTime.UtcNow)));
        } catch (Exception) {
            return Result.Failure<Models.User, DomainError>(ServiceErrors.UpdateError(id));
        }
    }

    /// <inheritdoc cref="IUserService.DeleteAsync"/>
    public async Task<Result<Models.User, DomainError>> DeleteAsync(int id) {
        try {
            return await ComprobarExistenciaAsync(id)
                .Tap(u => api.DeleteUserAsync(id))
                .Bind(u => repository.DeleteAsync(id))
                .Tap(u => cache.RemoveAsync(GetKeyUser(id)))
                .Tap(u => notificationService.Notificar( new Notification(
                    TypeNotification.Delete,
                    $"Se ha eliminado el usuario con ID {id}.",
                    DateTime.UtcNow)));  
        } catch (Exception) {
            return Result.Failure<Models.User, DomainError>(ServiceErrors.DeleteError(id));
        }
    }

    /// <inheritdoc cref="IUserService.ExportToJsonAsync"/>
    public async Task<Result<bool, DomainError>> ExportToJsonAsync() {
        var items = await repository.GetAllAsync();
        var res = await storage.ExportarJsonAsync(items.AsEnumerable(), Configuracion.UsersJsonPath);

        if (res.IsFailure) return Result.Failure<bool, DomainError>(ServiceErrors.ExportToJsonError(res.Error.Message));

        return Result.Success<bool, DomainError>(true);
    }
    
    /// <summary>
    /// Comprueba que existe el usuario con el id propocionado.
    /// </summary>
    /// <param name="id">Id del usuario.</param>
    /// <returns>Succes o failure dependiendo del resultado.</returns>
    private async Task<Result<Models.User, DomainError>> ComprobarExistenciaAsync(int id) {
        var res = await GetByIdAsync(id);
        return res.IsSuccess
            ? Result.Success<Models.User, DomainError>(res.Value)
            : Result.Failure<Models.User, DomainError>(UsersErrors.NotFoundError(id));
    }

    /// <summary>
    /// Devuelve una clave personalizada para la cache en base al id del objeto.
    /// </summary>
    private string GetKeyUser(int id) {
        return $"User:{id}";
    }
}