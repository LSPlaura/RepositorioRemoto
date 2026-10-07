using Microsoft.EntityFrameworkCore;
using RepositorioRemoto.Back.Mappers;
using RepositorioRemoto.Back.Models;

namespace RepositorioRemoto.Back.Entity;

/// <summary>
/// Contexto de la base de datos SQLite en Entity Framework Core.
/// Maneja la persistencia local de los datos de usuarios en un entorno SQLite.
/// </summary>
/// <param name="options">Opciones de configuración de conexión y comportamiento para el DbContext.</param>
public class AppDbContextSqlite(DbContextOptions<AppDbContextSqlite> options) : DbContext(options) {

    /// <summary>
    /// Conjunto de datos (DbSet) para acceder y gestionar la tabla de usuarios.
    /// </summary>
    public DbSet<User> Users => Set<User>();
    
    /// <summary>
    /// Configura el modelo de datos y sus mapeos mediante Fluent API.
    /// Define restricciones de columna y conversiones personalizadas de tipos complejos (Address, Company) 
    /// a cadenas JSON para compatibilidad con SQLite.
    /// </summary>
    /// <param name="modelBuilder">El constructor de modelos utilizado para configurar las entidades de la base de datos.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<User>(entity => {
                entity.ToTable("users");
                
                entity.Property(e => e.Id);
                
                entity.Property(e => e.Name);
                
                entity.Property(e => e.UserName);
                
                entity.Property(e => e.Email)
                    .IsRequired();
                
                // Conversión explícita de objeto Address a string JSON para almacenamiento en SQLite
                entity.Property(e => e.Address)
                    .HasConversion(
                        address => address.ToJson(),
                        json => json.ToAddress()
                    );
                
                entity.Property(e => e.Phone)
                    .IsRequired();
                
                entity.Property(e => e.Website);
                
                // Conversión explícita de objeto Company a string JSON para almacenamiento en SQLite
                entity.Property(e => e.Company)
                    .HasConversion(
                        company => company.ToJson(),
                        json => json.ToCompany()
                    );
                
                // Auditoría y eliminación lógica (Soft Delete)
                entity.Property(e => e.CreateAt);
                entity.Property(e => e.UpdateAt);
                entity.Property(e => e.DeleteAt);
                entity.Property(e => e.IsDeleted);
            }
        );
    }
}