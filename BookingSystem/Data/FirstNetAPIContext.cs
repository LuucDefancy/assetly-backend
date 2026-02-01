using BookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Data
{
    public class FirstNetAPIContext : DbContext
    {

        public FirstNetAPIContext(DbContextOptions<FirstNetAPIContext> options) : base(options) { }

        public DbSet<Device> Devices { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Rental> Rentals { get; set; }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Device>(entity =>
            {
                entity.HasIndex(device => device.SerialNumber).IsUnique();

                entity.Property(device => device.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(user => user.Email).IsUnique();
                entity.HasIndex(user => user.Username).IsUnique();
                entity.Property(user => user.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<Rental>(entity =>
            {
                entity.Property(rental => rental.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(rental => rental.RentalDate).HasDefaultValueSql("GETDATE()");
            });

        } 
    }
}
