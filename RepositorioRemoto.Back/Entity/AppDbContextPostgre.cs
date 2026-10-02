using Microsoft.EntityFrameworkCore;
using RepositorioRemoto.Back.Models;

namespace RepositorioRemoto.Back.Entity;

/// <summary>
/// Contexto de la base de datos PostgreSQL en Entity Framework Core.
/// Maneja la persistencia y mapeo de los datos de los usuarios.
/// </summary>
/// <param name="options">Opciones de configuración de conexión y comportamiento para el DbContext.</param>
public class AppDbContextPostgre(DbContextOptions<AppDbContextPostgre> options) : DbContext(options) {

    /// <summary>
    /// Conjunto de datos (DbSet) para acceder y gestionar la tabla de usuarios.
    /// </summary>
    public DbSet<User> Users => Set<User>();
    
    /// <summary>
    /// Configura el modelo de datos y sus mapeos mediante Fluent API.
    /// Define restricciones, tablas, columnas y tipos complejos (ej. columnas JSON para Address y Company).
    /// </summary>
    /// <param name="modelBuilder">El constructor de modelos utilizado para configurar las entidades de la base de datos.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<User>(entity => {
                entity.ToTable("users");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Name)
                    .HasMaxLength(50);
                
                entity.Property(e => e.UserName)
                    .HasMaxLength(30);
                
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(254);
                
                // Mapeo de tipo complejo a documento JSON en PostgreSQL
                entity.OwnsOne(e => e.Address, address => {
                    address.ToJson();
                    address.OwnsOne(e => e.Geo);
                });
                
                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(15);
                
                entity.Property(e => e.Website)
                    .HasMaxLength(250);
                
                // Mapeo de tipo complejo a documento JSON en PostgreSQL
                entity.OwnsOne(e => e.Company, company => {
                    company.ToJson();
                });
                
                // Auditoría y eliminación lógica (Soft Delete)
                entity.Property(e => e.CreateAt);
                entity.Property(e => e.UpdateAt);
                entity.Property(e => e.DeleteAt);
                entity.Property(e => e.IsDeleted);
            }
        );
    }
}