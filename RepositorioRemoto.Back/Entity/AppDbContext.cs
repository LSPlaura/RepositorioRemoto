using Microsoft.EntityFrameworkCore;
using RepositorioRemoto.Back.Models;

namespace RepositorioRemoto.Back.Entity;

/// <summary>
/// DbContext para entity framework core para la base de datos local de usuarios.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) {

    /// <summary>
    /// DbSet para la tabla de usuarios.
    /// </summary>
    public DbSet<User> Users => Set<User>();
    
    /// <summary>
    /// Configuracion del modelo para FluentApi
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<User>(entity => {
                entity.ToTable("users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
            }
        );
    }
}