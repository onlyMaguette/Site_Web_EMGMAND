using EMGMAND.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EMGMAND.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Car> Cars { get; set; }
        public DbSet<CarBrand> CarBrands { get; set; } // Ajout de la table des marques de voitures

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration de l'entité CarBrand
            modelBuilder.Entity<CarBrand>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            // Configuration de l'entité Car
            modelBuilder.Entity<Car>(entity =>
            {
                entity.Property(e => e.Model)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                entity.Property(e => e.Year)
                    .IsRequired()
                    .HasColumnType("int");

                entity.Property(e => e.IsSold)
                    .IsRequired();

                // Définir la relation entre Car et CarBrand
                entity.HasOne(e => e.Brand)
                      .WithMany() // Une marque peut être associée à plusieurs voitures
                      .HasForeignKey(e => e.BrandId) // Clé étrangère
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
