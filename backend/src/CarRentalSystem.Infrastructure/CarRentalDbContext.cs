using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Infrastructure;

public class CarRentalDbContext : DbContext
{
    public CarRentalDbContext()
    {
    }
    
    public CarRentalDbContext(DbContextOptions<CarRentalDbContext> options) : base(options)
    {
    }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<CarCategoryConfig> CarCategoryConfigs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Rental>(entity =>
        {
            entity.HasKey(e => e.BookingNumber);
            entity.Property(e => e.BookingNumber).HasMaxLength(50);
            entity.Property(e => e.RegistrationNumber).HasMaxLength(20).IsRequired();
            entity.Property(e => e.CustomerSocialSecurityNumber).HasMaxLength(20).IsRequired();
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
            
            entity.HasOne(e => e.CarCategory)
                .WithMany()
                .HasForeignKey(e => e.CarCategoryId);
        });

        modelBuilder.Entity<CarCategoryConfig>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Category).IsRequired();
            entity.Property(e => e.BaseDayRental).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.BaseKmPrice).HasColumnType("decimal(18,2)").IsRequired();
        });

        // Seed data
        modelBuilder.Entity<CarCategoryConfig>().HasData(
            new CarCategoryConfig { Id = 1, Category = CarCategory.SmallCar, BaseDayRental = 300, BaseKmPrice = 0 },
            new CarCategoryConfig { Id = 2, Category = CarCategory.Combi, BaseDayRental = 500, BaseKmPrice = 2 },
            new CarCategoryConfig { Id = 3, Category = CarCategory.Truck, BaseDayRental = 800, BaseKmPrice = 3 }
        );
        
        // Seed some data in Rental for testing purposes
        modelBuilder.Entity<Rental>().HasData(
            new Rental
            {
                BookingNumber = "BK0001",
                RegistrationNumber = "REG123",
                CustomerSocialSecurityNumber = "SSN123456",
                CarCategoryId = 1,
                TotalPrice = 350.00m
            },
            new Rental
            {
                BookingNumber = "BK0002",
                RegistrationNumber = "REG456",
                CustomerSocialSecurityNumber = "SSN654321",
                CarCategoryId = 2,
                TotalPrice = 520.00m
            }
        );
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("xxx");
        }

#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging();
#endif
        
    }
}
