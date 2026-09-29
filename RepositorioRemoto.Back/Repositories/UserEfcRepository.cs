using CSharpFunctionalExtensions;
using RepositorioRemoto.Back.Errors;
using RepositorioRemoto.Back.Models;
using Serilog;
using Serilog.Core;

namespace RepositorioRemoto.Back.Repositories;

/// <inheritdoc cref="IUserRepository"/>
public class UserEfcRepository : IUserRepository {

    private readonly ILogger _logger = Log.ForContext<UserEfcRepository>();
    
    public Task<IEnumerable<User>> GetAllAsync() {
        throw new NotImplementedException();
    }

    public Task<Result<User, DomainError>> GetByIdAsync(int id) {
        throw new NotImplementedException();
    }

    public Task<Result<User, DomainError>> CreateAsync(User entity) {
        throw new NotImplementedException();
    }

    public Task<Result<User, DomainError>> UpdateAsync(int id, User entity) {
        throw new NotImplementedException();
    }

    public Task<Result<User, DomainError>> DeleteAsync(int id) {
        throw new NotImplementedException();
    }

    public Task<Result<bool, DomainError>> DeleteAllAsync(int id) {
        throw new NotImplementedException();
    }
}