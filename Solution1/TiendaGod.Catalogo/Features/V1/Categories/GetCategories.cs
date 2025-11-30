using Microsoft.EntityFrameworkCore;
using TiendaGod.Productos.Data;

namespace TiendaGod.Productos.Features.V1.Categories
{
    public static class GetCategories
    {
        public static void MapGetCategories(this RouteGroupBuilder group)
        {
            group.MapGet("/", async (ProductDbContext db) =>
            {
                var categories = await db.Categories.ToListAsync();
                return Results.Ok(categories);
            })
            .WithName("GetCategories")
            .WithSummary("Lista todas las categorías")
            .WithOpenApi();
        }
    }
}
