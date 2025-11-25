using TiendaGod.Productos.Data;
using TiendaGod.Productos.Models;

namespace TiendaGod.Productos.Features.V1
{
    public static class UpdateProduct
    {
        public static void MapUpdateProduct(this RouteGroupBuilder group)
        {
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
            })
            .WithName("UpdateProduct")
            .WithSummary("Actualiza un producto existente");
        }
    }
}
