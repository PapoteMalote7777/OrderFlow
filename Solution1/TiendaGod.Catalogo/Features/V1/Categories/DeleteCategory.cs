using Microsoft.EntityFrameworkCore;
using TiendaGod.Productos.Data;

namespace TiendaGod.Productos.Features.V1.Categories
{
    public static class DeleteCategory
    {
        public static RouteHandlerBuilder MapDeleteCategory(this RouteGroupBuilder group)
        {
            return group.MapDelete("/{id:int}", async (int id, ProductDbContext db) =>
            {
                var category = await db.Categories.FindAsync(id);
                if (category is null) return Results.NotFound();

                var hasProducts = await db.Products.AnyAsync(p => p.CategoryId == id);
                if (hasProducts) return Results.BadRequest("No se puede eliminar porque la categoría tiene productos asociados.");

                db.Categories.Remove(category);
                await db.SaveChangesAsync();
                return Results.NoContent();
            })
            .WithName("DeleteCategory")
            .WithSummary("Elimina una categoría")
            .WithOpenApi();
        }
    }
}
