using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Infrastructure.Data.Seeders;


internal static class SeedCategories
{
    public static void CategorySeeder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Electronics",
                Description = "Electronic devices and accessories"
            },
            new Category
            {
                Id = Guid.Parse("22222222-1111-1111-1111-111111111111"),
                Name = "Games",
                Description = "Video Games"
            },
            new Category
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Clothing",
                Description = "Apparel and fashion items"
            },
            new Category
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Books",
                Description = "Books and educational materials"
            },
            new Category
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "Home & Garden",
                Description = "Home improvement and garden supplies"
            }
        );

    }
}




