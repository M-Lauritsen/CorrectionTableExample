using CorrectionTableExample.Models;
using CorrectionTableExample.Models.CorrectionModels;
using Microsoft.EntityFrameworkCore;

namespace CorrectionTableExample.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Customer> Customers { get; set; }

    public DbSet<EntityCorrection<Product>> ProductCorrections { get; set; }
    public DbSet<EntityCorrection<Customer>> CustomerCorrections { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Generisk konfiguration for alle IEntity typer
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType).HasKey("Id");
            }
        }

        ConfigureEntityCorrection<Product>();
        ConfigureEntityCorrection<Customer>();

        // Konfiguration for EntityCorrection<T>
        void ConfigureEntityCorrection<T>() where T : class, IEntity
        {
            modelBuilder.Entity<EntityCorrection<T>>()
                .HasKey(ec => ec.Id);

            modelBuilder.Entity<EntityCorrection<T>>()
                .HasOne(ec => ec.OriginalEntity)
                .WithMany()
                .HasForeignKey(ec => ec.OriginalEntityId);

            modelBuilder.Entity<EntityCorrection<T>>()
                .OwnsMany(ec => ec.PropertyCorrections, pc =>
                {
                    pc.WithOwner().HasForeignKey("EntityCorrectionId");
                    pc.Property<int>("Id");
                    pc.HasKey("Id");
                });
        }
    }
}

