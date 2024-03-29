using Microsoft.EntityFrameworkCore;
using OrderApi.Entities;

namespace OrderApi.Data
{
    public class AppOrderContext : DbContext
    {
        public AppOrderContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Order>().HasOne(x => x.Customer).WithMany(x => x.Orders);
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderID);
        }

        public DbSet<DevTask> DevTasks { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Shipper> Shippers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Tag> Tags { get; set; }

    }
}
