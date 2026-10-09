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
    }
}
