    using CSharpFunctionalExtensions;
    using RepositorioRemoto.Back.Errors;

    namespace RepositorioRemoto.Back.Repositories.Common;

    /// <summary>
    /// Contrato generico Crud para el sistema.
    /// </summary>
    public interface ICrudRepositoryAsync<in TKey, TEntity> where TEntity : class {
        
        /// <summary>
        /// Devuelve todas las entidades del sistema.
        /// </summary>
        /// <returns>Enumerable de las entidades.</returns>
        Task<IEnumerable<TEntity>> GetAllAsync();
        
        /// <summary>
        /// Devuelve la entidad cuyo Id sea igual al proporcionado.
        /// </summary>
        /// <param name="id">Id de la entidad</param>
        /// <returns>En caso de existir la entidad y en caso contrario failure.</returns>
        Task<Result<TEntity, DomainError>> GetByIdAsync(TKey id);
        
        /// <summary>
        /// Crea y guarda en el almacen una nueva entidad.
        /// </summary>
        /// <param name="entity">Entidad nueva.</param>
        /// <returns>En caso de ser correcta la entidad y en caso contrario failure.</returns>
        Task<Result<TEntity, DomainError>> CreateAsync(TEntity entity);
        
        /// <summary>
        /// Actualiza una entidad ya existente en el sistema.
        /// </summary>
        /// <param name="id">Id de la entidad existente.</param>
        /// <param name="entity">Entidad existente actualizada.</param>
        /// <returns>En caso de ser correcta la entidad y en caso contrario failure.</returns>
        Task<Result<TEntity, DomainError>> UpdateAsync(TKey id, TEntity entity);
        
        /// <summary>
        /// ELimina la entidad del sistema.
        /// </summary>
        /// <param name="id">Id de la entidad existente.</param>
        /// <returns>En caso de ser correcta la entidad eliminada y failure en caso contrario.</returns>
        Task<Result<TEntity, DomainError>> DeleteAsync(TKey id);
        
        /// <summary>
        /// ELimina la entidad del sistema.
        /// </summary>
        /// <returns>True si se ha podido eliminar, failure en caso contrario.</returns>
        Task<Result<bool, DomainError>> DeleteAllAsync();
    }