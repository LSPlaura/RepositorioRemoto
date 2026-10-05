using CSharpFunctionalExtensions;
using RepositorioRemoto.Back.Cache.Common;
using RepositorioRemoto.Back.Config;
using RepositorioRemoto.Back.Dto.Users.Request;
using RepositorioRemoto.Back.Errors;
using RepositorioRemoto.Back.Errors.Service;
using RepositorioRemoto.Back.Errors.Users;
using RepositorioRemoto.Back.Infrastructure.Interfaces;
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
    INotificationService notificationService
    ) : IUserService, IScopedService {
    
    
    public async Task<Result<IEnumerable<Models.User>, DomainError>> GetAllAsync() {
        throw new NotImplementedException();
    }

    public async Task<Result<Models.User, DomainError>> GetByIdAsync(int id) {
        throw new NotImplementedException();
    }

    public async Task<Result<Models.User, DomainError>> CreateAsync(CreateUserRequest request) {
        throw new NotImplementedException();
    }

    public async Task<Result<Models.User, DomainError>> UpdateAsync(int id, UpdateUserRequest request) {
        throw new NotImplementedException();
    }

    public async Task<Result<Models.User, DomainError>> DeleteAsync(int id) {
        return await ComprobarExistenciaAsync(id)
            .Bind(u => repository.DeleteAsync(id))
            .Tap(u => cache.RemoveAsync(GetKeyUser(id)));        
    }

    public async Task<Result<bool, DomainError>> ExportToJsonAsync() {
        var items = await repository.GetAllAsync();
        var res = await storage.ExportarJsonAsync(items.AsEnumerable(), Configuracion.UsersJsonPath);

        if (res.IsFailure) return Result.Failure<bool, DomainError>(ServiceErrors.ExportToJsonError(res.Error.Message));

        return Result.Success<bool, DomainError>(true);
    }
    
    private async Task<Result<Models.User, DomainError>> ComprobarExistenciaAsync(int id) {
        var res = await repository.GetByIdAsync(id);
        return res.IsSuccess
            ? Result.Success<Models.User, DomainError>(res.Value)
            : Result.Failure<Models.User, DomainError>(UsersErrors.NotFoundError(id));
    }

    private string GetKeyUser(int id) {
        return $"User:{id}";
    }
}