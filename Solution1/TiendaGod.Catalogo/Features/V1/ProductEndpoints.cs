using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaGod.Productos.Data;
using TiendaGod.Productos.Models;

namespace TiendaGod.Productos.Features.V1
{
    public static class ProductEndpointsV1
    {
        public static RouteGroupBuilder MapProductEndpointsV1(this WebApplication app)
        {
            var group = app.MapGroup("/api/v1/products")
                           .WithTags("Products v1")
                           .WithOpenApi(); // ¡Esto sí funciona siempre!

            group.MapGet("/", async (ProductDbContext db) =>
            {
                return Results.Ok(await db.Products.ToListAsync());
            });

            group.MapGet("/{id:int}", async (int id, ProductDbContext db) =>
            {
                var product = await db.Products.FindAsync(id);
                return product is null ? Results.NotFound() : Results.Ok(product);
            });

            group.MapPost("/", async ([FromBody] Product product, ProductDbContext db) =>
            {
                db.Products.Add(product);
                await db.SaveChangesAsync();
                return Results.Created($"/api/v1/products/{product.Id}", product);
            });

            group.MapPut("/{id:int}", async (int id, Product update, ProductDbContext db) =>
            {
                var existing = await db.Products.FindAsync(id);
                if (existing is null) return Results.NotFound();

                existing.Name = update.Name;
                existing.Description = update.Description;
                existing.Price = update.Price;
                existing.Brand = update.Brand;

                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            group.MapDelete("/{id:int}", async (int id, ProductDbContext db) =>
            {
                var product = await db.Products.FindAsync(id);
                if (product is null) return Results.NotFound();

                db.Products.Remove(product);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            return group;
        }
    }
}
