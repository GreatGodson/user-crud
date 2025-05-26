using Microsoft.EntityFrameworkCore;
using MyFirstApi.Models;

namespace MyFirstApi.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Set Id as primary key explicitly (usually EF infers this automatically)
            _ = modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            // Ensure Id is unique at the database level (usually redundant for primary keys)
            _ = modelBuilder.Entity<User>()
                .HasIndex(u => u.Id)
                .IsUnique();
        }
    }
}