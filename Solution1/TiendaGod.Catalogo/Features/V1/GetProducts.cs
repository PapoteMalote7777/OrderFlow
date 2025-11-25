using Microsoft.EntityFrameworkCore;
using TiendaGod.Productos.Data;

namespace TiendaGod.Productos.Features.V1
{
    public static class GetProducts
    {
        public static void MapGetProducts(this RouteGroupBuilder group)
        {
            group.MapGet("/", async (ProductDbContext db) =>
            {
                var products = await db.Products.ToListAsync();
                return Results.Ok(products);
            })
            .WithName("GetProducts")
            .WithSummary("Lista todos los productos")
            .WithOpenApi();
        }
    }
}
