using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DataAccessLayer.Data
{
    public class CollectorsKeepDbContext : IdentityDbContext<ApplicationUser>
    {
        public CollectorsKeepDbContext(DbContextOptions<CollectorsKeepDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<UserKeepItem> UserKeepItems { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>()
                   .Property(p => p.Price)
                   .HasPrecision(18, 2);

            builder.Entity<OrderItem>()
                   .Property(oi => oi.UnitPrice)
                   .HasPrecision(18, 2);


            builder.Entity<Wishlist>()
                   .HasOne(w => w.Customer)
                   .WithMany(c => c.Wishlists)
                   .HasForeignKey(w => w.CustomerID)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WishlistItem>()
                   .HasIndex(wi => new { wi.WishlistID, wi.ProductID })
                   .IsUnique();

            builder.Entity<WishlistItem>()
                   .HasOne(wi => wi.Wishlist)
                   .WithMany(w => w.WishlistItems)
                   .HasForeignKey(wi => wi.WishlistID)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WishlistItem>()
                   .HasOne(wi => wi.Product)
                   .WithMany(p => p.WishlistItems)
                   .HasForeignKey(wi => wi.ProductID)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Inventory>()
                   .HasOne(i => i.Product)
                   .WithOne(p => p.Inventory)
                   .HasForeignKey<Inventory>(i => i.ProductID)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Inventory>()
                   .ToTable(t => t.HasCheckConstraint("CK_Inventory_Quantity", "Quantity >= 0"));

            builder.Entity<Address>()
                   .HasOne(a => a.Customer)
                   .WithMany(c => c.Addresses)
                   .HasForeignKey(a => a.CustomerID)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Order>()
                   .HasOne(o => o.Customer)
                   .WithMany(c => c.Orders)
                   .HasForeignKey(o => o.CustomerID);

            builder.Entity<OrderItem>()
                   .HasOne(oi => oi.Order)
                   .WithMany(o => o.OrderItems)
                   .HasForeignKey(oi => oi.OrderID);

            builder.Entity<OrderItem>()
                   .HasOne(oi => oi.Product)
                   .WithMany(p => p.OrderItems)
                   .HasForeignKey(oi => oi.ProductID)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserKeepItem>()
    .HasOne(k => k.User)
    .WithMany()
    .HasForeignKey(k => k.UserId)
    .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserKeepItem>()
                .HasOne(k => k.Product)
                .WithMany()
                .HasForeignKey(k => k.ProductID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserKeepItem>()
                .Property(k => k.ItemValue)
                .HasPrecision(18, 2);


        }
    }
}
