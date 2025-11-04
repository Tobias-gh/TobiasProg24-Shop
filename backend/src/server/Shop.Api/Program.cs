using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Shop.Application.Services.Cart;
using Shop.Application.Services.Category;
using Shop.Application.Services.Product;
using Shop.Domain.Interfaces;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Shop API", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});


builder.Services.AddDbContext<ShopDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICartService, CartService>();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // UI at /swagger
}


app.UseCors("DevCors");

app.UseHttpsRedirection();
app.MapControllers();


app.Run();

