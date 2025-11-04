using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Infrastructure.Data.Seeders;

internal static class ProductSeeder
{
    public static void SeedProducts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Name = "Smartphone",
                Description = "Latest model smartphone with advanced features",
                Price = 699.99m,
                Stock = 50,
                CategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111")
            },
            new Product
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Name = "Jeans",
                Description = "Comfortable and stylish denim jeans",
                Price = 49.99m,
                Stock = 100,
                CategoryId = Guid.Parse("22222222-2222-2222-2222-222222222222")
            },
            new Product
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                Name = "Science Fiction Novel",
                Description = "A thrilling science fiction novel set in the future",
                Price = 19.99m,
                Stock = 200,
                CategoryId = Guid.Parse("33333333-3333-3333-3333-333333333333")
            },
            new Product
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                Name = "Garden Tools Set",
                Description = "Complete set of garden tools for all your gardening needs",
                Price = 89.99m,
                Stock = 75,
                CategoryId = Guid.Parse("44444444-4444-4444-4444-444444444444")
            }
        );
    }

}


    

