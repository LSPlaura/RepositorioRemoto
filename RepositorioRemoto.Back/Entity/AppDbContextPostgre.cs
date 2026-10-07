using Microsoft.EntityFrameworkCore;
using RepositorioRemoto.Back.Mappers;
using RepositorioRemoto.Back.Models;

namespace RepositorioRemoto.Back.Entity;

public class AppDbContextPostgre(DbContextOptions<AppDbContextPostgre> options) : DbContext(options) {

    public DbSet<User> Users => Set<User>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<User>(entity => {
                entity.ToTable("users");

                entity.Property(e => e.Id);
                
                entity.Property(e => e.Name)
                    .HasMaxLength(50);
                
                entity.Property(e => e.UserName)
                    .HasMaxLength(30);
                
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(254);
                
                entity.Property(e => e.Address)
                    .HasConversion(
                        address => address.ToJson(),
                        json => json.ToAddress()
                    )
                    .HasColumnType("jsonb");
                
                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(15);
                
                entity.Property(e => e.Website)
                    .HasMaxLength(250);
                
                entity.Property(e => e.Company)
                    .HasConversion(
                        company => company.ToJson(),
                        json => json.ToCompany()
                    )
                    .HasColumnType("jsonb");
                
                // Auditoría y eliminación lógica
                entity.Property(e => e.CreateAt);
                entity.Property(e => e.UpdateAt);
                entity.Property(e => e.DeleteAt);
                entity.Property(e => e.IsDeleted);
            }
        );
    }
}