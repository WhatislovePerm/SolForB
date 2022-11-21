using System;
using Microsoft.EntityFrameworkCore;
using solforbTest.Data.Entity;

namespace solforbTest.Data.DataContext
{
    public class ApplicationDataContext : DbContext
    {
        public ApplicationDataContext(DbContextOptions<ApplicationDataContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Order> Order { get; set; }

        public DbSet<OrderItem> OrderItem { get; set; }

        public DbSet<Provider> Provider { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasIndex(p => new { p.Number, p.ProviderId }).IsUnique();
        }
    }
}

