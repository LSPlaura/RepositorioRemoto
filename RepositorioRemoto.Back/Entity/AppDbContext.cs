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
                entity.OwnsOne(e => e.Address, address => {
                    address.Property(e => e.Street)
                        .HasMaxLength(50);
                    address.Property(e => e.Suite)
                        .HasMaxLength(50);
                    address.Property(e => e.City)
                        .HasMaxLength(50);
                    address.Property(e => e.ZipCode)
                        .HasMaxLength(50);
                    address.OwnsOne(e => e.Geo, geo => {
                        geo.Property(e => e.Lat)
                            .HasMaxLength(50);
                        geo.Property(e => e.Lng)
                            .HasMaxLength(50);
                    });
                });
                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(8);
                entity.Property(e => e.Website)
                    .HasMaxLength(50);
                entity.OwnsOne(e => e.Company, company=> {
                    company.Property(e => e.Name)
                        .HasMaxLength(50);
                    company.Property(e => e.CatchPhrase)
                        .HasMaxLength(50);
                    company.Property(e => e.Bs)
                        .HasMaxLength(50);
                });
                entity.Property(e => e.CreateAt);
                entity.Property(e => e.UpdateAt);
                entity.Property(e => e.DeleteAt);
                entity.Property(e => e.IsDeleted);
            }
        );
    }
}