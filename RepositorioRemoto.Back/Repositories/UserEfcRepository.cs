using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using RepositorioRemoto.Back.Entity;
using RepositorioRemoto.Back.Errors;
using RepositorioRemoto.Back.Errors.Repository;
using RepositorioRemoto.Back.Errors.Users;
using RepositorioRemoto.Back.Infrastructure.Interfaces;
using RepositorioRemoto.Back.Models;
using Serilog;

namespace RepositorioRemoto.Back.Repositories;

/// <inheritdoc cref="IUserRepository"/>
public class UserEfcRepository(DbContext context) : IUserRepository, IScopedService
{
    private readonly ILogger _logger = Log.ForContext<UserEfcRepository>();
    private readonly DbContext _context = context;

    /// <inheritdoc cref="IUserRepository.GetAllAsync"/>
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Set<User>()
            .AsNoTracking()
            .OrderBy(u => u.Id)
            .ToListAsync();
    }

    /// <inheritdoc cref="IUserRepository.GetByIdAsync"/>
    public async Task<Result<User, DomainError>> GetByIdAsync(int id)
    {
        var entity = await _context.Set<User>().FindAsync(id);
        if (entity is null)
        {
            _logger.Debug($"Error al intentar encontrar la entidad con el id: {id}.");
            return Result.Failure<User, DomainError>(UsersErrors.NotFoundError(id));
        }

        _logger.Debug($"Se ha encontrado con exito la entidad con el id: {id}.");
        return Result.Success<User, DomainError>(entity);
    }

    public async Task<Result<User, DomainError>> CreateAsync(User entity)
    {
        try
        {
            var entry = await _context.Set<User>().AddAsync(entity);
            
            entry.Property(u => u.Id).IsTemporary = false;

            await _context.SaveChangesAsync();
            
            await _context.Database.ExecuteSqlRawAsync(@"
            SELECT setval(
                pg_get_serial_sequence('users', 'Id'), 
                COALESCE((SELECT MAX(""Id"") FROM users), 1)
            );
        ");

            _logger.Debug("Se ha registrado correctamente la nueva entidad.");
            return Result.Success<User, DomainError>(entity);
        }
        catch (Exception ex)
        {
            _logger.Debug(ex, "Error al intentar registrar la entidad.");
            return Result.Failure<User, DomainError>(RepositoryErrors.CreationError());
        }
    }

    public async Task<Result<User, DomainError>> UpdateAsync(int id, User entity)
    {
        try
        {
            var user = await _context.Set<User>().FindAsync(id);
            if (user is null)
            {
                _logger.Debug($"Error al intentar encontrar la entidad con el id: {id}.");
                return Result.Failure<User, DomainError>(UsersErrors.NotFoundError(id));
            }

            if (user.IsDeleted)
            {
                _logger.Debug("Error la entidad ya esta borrada.");
                return Result.Failure<User, DomainError>(RepositoryErrors.UpdatedError());
            }

            if (entity.Address is null || entity.Company is null)
            {
                _logger.Debug("Error: Address o Company no pueden ser nulos al actualizar.");
                return Result.Failure<User, DomainError>(RepositoryErrors.UpdatedError());
            }

            var userEntry = _context.Entry(user);
            if (userEntry is null)
            {
                _context.Set<User>().Update(entity);
            }
            else
            {
                _context.Entry(user).CurrentValues.SetValues(entity);
                _context.Entry(user).Property(u => u.Id).IsModified = false;
            }

            await _context.SaveChangesAsync();

            _logger.Debug($"Se ha actualizado con exito la entidad con el id: {id}.");
            return Result.Success<User, DomainError>(entity);
        }
        catch (Exception ex)
        {
            _logger.Debug(ex, $"Error al intentar actualizar la entidad.");
            return Result.Failure<User, DomainError>(RepositoryErrors.UpdatedError());
        }
    }

    public async Task<Result<User, DomainError>> DeleteAsync(int id)
    {
        try
        {
            var user = await _context.Set<User>().FindAsync(id);
            if (user is null)
            {
                _logger.Debug($"Error al intentar encontrar la entidad con el id: {id}.");
                return Result.Failure<User, DomainError>(UsersErrors.NotFoundError(id));
            }

            if (user.IsDeleted)
            {
                _logger.Debug("Error la entidad ya esta borrada.");
                return Result.Failure<User, DomainError>(RepositoryErrors.UpdatedError());
            }

            _context.Set<User>().Remove(user);
            await _context.SaveChangesAsync();

            _logger.Debug($"Se ha eliminado con exito la entidad con el id: {id}.");
            return Result.Success<User, DomainError>(user);
        }
        catch (Exception ex)
        {
            _logger.Debug(ex, $"Error al intentar eliminar la entidad.");
            return Result.Failure<User, DomainError>(RepositoryErrors.DeletedError());
        }
    }

    public async Task<Result<bool, DomainError>> DeleteAllAsync()
    {
        try
        {
            await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE users RESTART IDENTITY CASCADE;");

            _logger.Debug("Se han eliminado todas las entidades registradas.");
            return Result.Success<bool, DomainError>(true);
        }
        catch (Exception ex)
        {
            _logger.Debug(ex, "Error al intentar eliminar todas las entidades.");
            return Result.Failure<bool, DomainError>(RepositoryErrors.DeletedError());
        }
    }
}