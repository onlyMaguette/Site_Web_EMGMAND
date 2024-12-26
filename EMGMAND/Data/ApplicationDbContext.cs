using EMGMAND.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EMGMAND.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Car> Cars { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration de l'entité Car
            modelBuilder.Entity<Car>(entity =>
            {
                entity.Property(e => e.Brand)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Model)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                entity.Property(e => e.Year)
                    .IsRequired();

                entity.Property(e => e.IsSold)
                    .IsRequired();

                entity.Property(e => e.IsAvailable)
                    .IsRequired();
            });
        }
    }
}
