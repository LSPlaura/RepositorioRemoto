using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using RepositorioRemoto.Back.Entity;
using RepositorioRemoto.Back.Errors;
using RepositorioRemoto.Back.Models;
using Serilog;
using Serilog.Core;

namespace RepositorioRemoto.Back.Repositories;

/// <inheritdoc cref="IUserRepository"/>
public class UserEfcRepository(AppDbContext context) : IUserRepository {

    private readonly ILogger _logger = Log.ForContext<UserEfcRepository>();
    private readonly AppDbContext _context = context;
    
    /// <inheritdoc cref="IUserRepository.GetAllAsync"/>
    public Task<IEnumerable<User>> GetAllAsync() {
        var entities = _context.Users
            .OrderBy(u => u.Id)
            .ToListAsync();
        return 
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