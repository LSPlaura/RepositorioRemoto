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
public class UserEfcRepository(AppDbContextPostgre contextPostgre) : IUserRepository, IScopedService{

    private readonly ILogger _logger = Log.ForContext<UserEfcRepository>();
    private readonly AppDbContextPostgre _contextPostgre = contextPostgre;
    
    /// <inheritdoc cref="IUserRepository.GetAllAsync"/>
    public async Task<IEnumerable<User>> GetAllAsync() {
        var entities = await _contextPostgre.Users
            .OrderBy(u => u.Id)
            .ToListAsync();
        return entities;
    }

    /// <inheritdoc cref="IUserRepository.GetByIdAsync" />
    public async Task<Result<User, DomainError>> GetByIdAsync(int id) {
        var entity = await _contextPostgre.Users.FindAsync(id);
        if (entity is null) {
            _logger.Debug($"Error al intentar encontrar la entidad con el id: {id}.");
            return Result.Failure<User, DomainError>(UsersErrors.NotFoundError(id));
        }
        _logger.Debug($"Se ha encontrado con exito la entidad con el id: {id}.");
        return Result.Success<User, DomainError>(entity);
    }

    public async Task<Result<User, DomainError>> CreateAsync(User entity) {
        try {
            _contextPostgre.Users.Add(entity);
            await _contextPostgre.SaveChangesAsync();
            
            _logger.Debug($"Se ha registrado correctamente la nueva entidad.");
            return Result.Success<User, DomainError>(entity);
        } catch ( Exception ) {
            _logger.Debug($"Error al intentar registrar la entidad.");
            return Result.Failure<User, DomainError>(RepositoryErrors.CreationError());
        }
    }

    public async Task<Result<User, DomainError>> UpdateAsync(int id, User entity) {
        var user = await _contextPostgre.Users.FindAsync(id);
        if (user is null) {
            _logger.Debug($"Error al intentar encontrar la entidad con el id: {id}.");
            return Result.Failure<User, DomainError>(UsersErrors.NotFoundError(id));
        }
        if(user.IsDeleted) {
            _logger.Debug("Error la entidad ya esta borrada.");
            return Result.Failure<User, DomainError>(RepositoryErrors.UpdatedError());
        }

        try {
            user = user with {
                Id = id,  
                Name = entity.Name,
                UserName = entity.UserName,
                Email = entity.Email,
                Address = entity.Address,
                Phone = entity.Phone,
                Website = entity.Website,
                Company = entity.Company,
                CreateAt = entity.CreateAt,
                UpdateAt = entity.UpdateAt,
                DeleteAt = entity.DeleteAt,
                IsDeleted = entity.IsDeleted
            };
            await _contextPostgre.SaveChangesAsync();
            
            _logger.Debug($"Se ha actualizado con exito la entidad con el id: {id}.");
            return Result.Success<User, DomainError>(user);
        } catch (Exception) {
            _logger.Debug($"Error al intentar actualizar la entidad.");
            return Result.Failure<User, DomainError>(RepositoryErrors.UpdatedError());
        }
    }

    public async Task<Result<User, DomainError>> DeleteAsync(int id) {
        var user = await _contextPostgre.Users.FindAsync(id);
        if (user is null) {
            _logger.Debug($"Error al intentar encontrar la entidad con el id: {id}.");
            return Result.Failure<User, DomainError>(UsersErrors.NotFoundError(id));
        }
        if(user.IsDeleted) {
            _logger.Debug("Error la entidad ya esta borrada.");
            return Result.Failure<User, DomainError>(RepositoryErrors.UpdatedError());
        }

        try {
            _contextPostgre.Users.Remove(user);
            await _contextPostgre.SaveChangesAsync();
            
            _logger.Debug($"Se ha eliminado con exito la entidad con el id: {id}.");
            return Result.Success<User, DomainError>(user);
        } catch (Exception ) {
            _logger.Debug($"Error al intentar eliminar la entidad.");
            return Result.Failure<User, DomainError>(RepositoryErrors.DeletedError());
        }
    }

    public async Task<Result<bool, DomainError>> DeleteAllAsync() {
        try {
            var entities = await _contextPostgre.Users.ToListAsync();
            _contextPostgre.Users.RemoveRange(entities);
            await _contextPostgre.SaveChangesAsync();
            
            _logger.Debug("Se han eliminado todas las entidades registrados.");
            return Result.Success<bool, DomainError>(true);
        } catch (Exception) {
            _logger.Debug($"Error al intentar eliminar todas las entidades.");
            return Result.Failure<bool, DomainError>(RepositoryErrors.DeletedError());
        }
    }
}