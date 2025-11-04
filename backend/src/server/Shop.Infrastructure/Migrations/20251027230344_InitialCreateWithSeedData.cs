using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Shop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateWithSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", maxLength: 500, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", maxLength: 500, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Stock = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Electronic devices and accessories", "Electronics" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Apparel and fashion items", "Clothing" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Books and educational materials", "Books" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Home improvement and garden supplies", "Home & Garden" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Description", "Name", "Price", "Stock" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), "Latest model smartphone with advanced features", "Smartphone", 699.99m, 50 },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("22222222-2222-2222-2222-222222222222"), "Comfortable and stylish denim jeans", "Jeans", 49.99m, 100 },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new Guid("33333333-3333-3333-3333-333333333333"), "A thrilling science fiction novel set in the future", "Science Fiction Novel", 19.99m, 200 },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new Guid("44444444-4444-4444-4444-444444444444"), "Complete set of garden tools for all your gardening needs", "Garden Tools Set", 89.99m, 75 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
