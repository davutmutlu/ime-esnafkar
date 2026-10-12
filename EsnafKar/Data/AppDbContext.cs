using EsnafKar.Models;
using Microsoft.EntityFrameworkCore;

namespace EsnafKar.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Isletme> Isletmeler { get; set; }
        public DbSet<GiderKategorisi> GiderKategorileri { get; set; }
        public DbSet<Gider> Giderler { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Gider>()
                .HasOne(g => g.GiderKategorisi)
                .WithMany()
                .HasForeignKey(g => g.GiderKategorisiId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GiderKategorisi>().HasData(
                new GiderKategorisi { Id = 1, Ad = "Kira" },
                new GiderKategorisi { Id = 2, Ad = "Maaş" },
                new GiderKategorisi { Id = 3, Ad = "Elektrik" },
                new GiderKategorisi { Id = 4, Ad = "Su" },
                new GiderKategorisi { Id = 5, Ad = "Doğalgaz" },
                new GiderKategorisi { Id = 6, Ad = "İnternet / Telefon" },
                new GiderKategorisi { Id = 7, Ad = "Vergi / Harç" },
                new GiderKategorisi { Id = 8, Ad = "Bakım / Onarım" },
                new GiderKategorisi { Id = 9, Ad = "Diğer" }
            );
        }
    }
}