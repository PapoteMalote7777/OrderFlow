using Microsoft.AspNetCore.Mvc;
using TiendaGod.Productos.Data;
using TiendaGod.Productos.Models;

namespace TiendaGod.Productos.Features.V1
{
    public static class CreateProduct
    {
        public static void MapCreateProduct(this RouteGroupBuilder group)
        {
            group.MapPost("/", async ([FromBody] Product product, ProductDbContext db) =>
            {
                db.Products.Add(product);
                await db.SaveChangesAsync();
                return Results.Created($"/api/v1/products/{product.Id}", product);
            })
            .WithName("CreateProduct")
            .WithSummary("Crea un nuevo producto");
        }
    }
}
