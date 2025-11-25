using TiendaGod.Productos.Data;

namespace TiendaGod.Productos.Features.V1
{
    public static class GetProductById
    {
        public static void MapGetProductById(this RouteGroupBuilder group)
        {
            group.MapGet("/{id:int}", async (int id, ProductDbContext db) =>
            {
                var product = await db.Products.FindAsync(id);
                return product is null ? Results.NotFound() : Results.Ok(product);
            })
            .WithName("GetProductById")
            .WithSummary("Obtiene un producto por ID")
            .WithOpenApi();
        }
    }
}
