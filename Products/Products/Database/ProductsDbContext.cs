using Microsoft.EntityFrameworkCore;
using Products.Database.Entities;

namespace Products.Database
{
    public class ProductsDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer("Data Source=192.168.1.11,1433;Persist Security Info=True;User ID=sa;Password=Password01;Database=ProductsDb");
    }
}
