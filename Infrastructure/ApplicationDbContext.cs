using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(p => p.Id);
                entity.Property(e => e.Id);// Tự động sinh Guid khi thêm mới
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(c => c.CreatedOn)
                .HasDefaultValueSql("GETUTCDATE()");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(p => p.Id);
                entity.Property(e => e.Id);// Tự động sinh Guid khi thêm mới
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(c => c.CreatedOn)
                .HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}
