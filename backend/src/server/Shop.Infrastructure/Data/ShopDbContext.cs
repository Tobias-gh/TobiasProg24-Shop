using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities;
using Shop.Infrastructure.Data.Seeders;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Infrastructure.Data;

    public sealed class ShopDbContext(DbContextOptions<ShopDbContext> options) : DbContext(options)
    {
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShopDbContext).Assembly);

        SeedCategories.CategorySeeder(modelBuilder);
        ProductSeeder.SeedProducts(modelBuilder);
    }
    //public DbSet<Cart> Carts => Set<Cart>();
    //public DbSet<Order> Orders => Set<Order>();

    //DbSet<CartItem> CartItems => Set<CartItem>();


}

        



