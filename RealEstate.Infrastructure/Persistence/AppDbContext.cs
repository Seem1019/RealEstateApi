using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }
        public DbSet<PropertyTrace> PropertyTraces { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships
            modelBuilder.Entity<Property>()
                .HasOne(p => p.Owner)
                .WithMany(o => o.Properties)
                .HasForeignKey(p => p.IdOwner)
                .OnDelete(DeleteBehavior.Cascade); // Or Restrict

            modelBuilder.Entity<PropertyImage>()
                .HasOne(pi => pi.Property)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.IdProperty)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PropertyTrace>()
                .HasOne(pt => pt.Property)
                .WithMany(p => p.Traces)
                .HasForeignKey(pt => pt.IdProperty)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance
            modelBuilder.Entity<Property>().HasIndex(p => p.Price);
            modelBuilder.Entity<Property>().HasIndex(p => p.Year);
            modelBuilder.Entity<Property>().HasIndex(p => p.Address);
            modelBuilder.Entity<Property>().HasIndex(p => p.CodeInternal).IsUnique();
            modelBuilder.Entity<Property>().HasIndex(p => p.Name);

            base.OnModelCreating(modelBuilder);
        }
    }
}