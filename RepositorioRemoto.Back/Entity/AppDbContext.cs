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
                    .HasMaxLength(100);
                entity.Property(e => e.UserName)
                    .HasMaxLength(50);
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Address );
                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(8);
                entity.Property(e => e.Website)
                    .HasMaxLength(50);
                entity.Property(e => e.Company)
                    .HasConversion<string>();
                entity.Property(e => e.CreateAt);
                entity.Property(e => e.UpdateAt);
                entity.Property(e => e.DeleteAt);
                entity.Property(e => e.IsDeleted);
            }
        );
    }
}