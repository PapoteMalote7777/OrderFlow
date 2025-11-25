using TiendaGod.Productos.Data;
using TiendaGod.Productos.DTO;
using TiendaGod.Productos.Models;

namespace TiendaGod.Productos.Features.V1
{
    public static class UpdateProduct
    {
        public static void MapUpdateProduct(this RouteGroupBuilder group)
        {
            group.MapPut("/{id:int}", async (int id, ProductUpdateDto dto, ProductDbContext db) =>
            {
                var existing = await db.Products.FindAsync(id);
                if (existing is null) return Results.NotFound();

                existing.Name = dto.Name;
                existing.Description = dto.Description;
                existing.Price = dto.Price;
                existing.Brand = dto.Brand;

                await db.SaveChangesAsync();
                return Results.NoContent();
            })
            .WithName("UpdateProduct")
            .WithSummary("Actualiza un producto existente")
            .WithOpenApi();

        }
    }
}
