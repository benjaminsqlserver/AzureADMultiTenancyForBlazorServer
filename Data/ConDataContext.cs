using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using SimplifiedNorthwind.Models.ConData;

namespace SimplifiedNorthwind.Data
{
    public partial class ConDataContext : DbContext
    {
        public ConDataContext()
        {
        }

        public ConDataContext(DbContextOptions<ConDataContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<SimplifiedNorthwind.Models.ConData.Order>()
              .HasOne(i => i.Customer)
              .WithMany(i => i.Orders)
              .HasForeignKey(i => i.CustomerId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<SimplifiedNorthwind.Models.ConData.OrderItem>()
              .HasOne(i => i.Order)
              .WithMany(i => i.OrderItems)
              .HasForeignKey(i => i.OrderId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<SimplifiedNorthwind.Models.ConData.OrderItem>()
              .HasOne(i => i.Product)
              .WithMany(i => i.OrderItems)
              .HasForeignKey(i => i.ProductId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<SimplifiedNorthwind.Models.ConData.Product>()
              .HasOne(i => i.Supplier)
              .WithMany(i => i.Products)
              .HasForeignKey(i => i.SupplierId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<SimplifiedNorthwind.Models.ConData.Order>()
              .Property(p => p.OrderDate)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<SimplifiedNorthwind.Models.ConData.Order>()
              .Property(p => p.TotalAmount)
              .HasDefaultValueSql(@"((0))");

            builder.Entity<SimplifiedNorthwind.Models.ConData.OrderItem>()
              .Property(p => p.Quantity)
              .HasDefaultValueSql(@"((1))");

            builder.Entity<SimplifiedNorthwind.Models.ConData.Product>()
              .Property(p => p.UnitPrice)
              .HasDefaultValueSql(@"((0))");
            this.OnModelBuilding(builder);
        }

        public DbSet<SimplifiedNorthwind.Models.ConData.Customer> Customers { get; set; }

        public DbSet<SimplifiedNorthwind.Models.ConData.Order> Orders { get; set; }

        public DbSet<SimplifiedNorthwind.Models.ConData.OrderItem> OrderItems { get; set; }

        public DbSet<SimplifiedNorthwind.Models.ConData.Product> Products { get; set; }

        public DbSet<SimplifiedNorthwind.Models.ConData.SolutionUser> SolutionUsers { get; set; }

        public DbSet<SimplifiedNorthwind.Models.ConData.Supplier> Suppliers { get; set; }
    }
}