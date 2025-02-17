using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace webapi_core.Models
{
    public class ShopContext : DbContext
    {
        public ShopContext(DbContextOptions<ShopContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasMany(c => c.Products).WithOne(a => a.Category).HasForeignKey(a => a.CategoryId);
            modelBuilder.Entity<Order>().HasMany(c => c.Products);
            modelBuilder.Entity<User>().HasMany(u => u.Orders).WithOne(o => o.User).HasForeignKey(o => o.UserId);
            modelBuilder.Seed();

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<User> Users { get; set; }

    }
}