using Microsoft.EntityFrameworkCore;
using Orders.Database.Entities;

namespace Orders.Database
{
    public class OrdersDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer("Data Source=192.168.1.11,1433;Persist Security Info=True;User ID=sa;Password=Password01;Database=OrdersDb");
    }
}
