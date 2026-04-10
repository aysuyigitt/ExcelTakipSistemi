using ExcelSatısVerileri.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExcelSatısVerileri.Context
{
    public class ApiContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=localhost;Database=ExcelSatislarDb;Uid=sa;Pwd=aysu123;TrustServerCertificate=True;");
        }

        public DbSet<Satis> Satislar { get; set; }
    }
}
