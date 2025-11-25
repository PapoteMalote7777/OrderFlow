using TiendaGod.Productos.Data;

namespace TiendaGod.Productos.Features.V1
{
    public static class DeleteProduct
    {
        public static void MapDeleteProduct(this RouteGroupBuilder group)
        {
            group.MapDelete("/{id:int}", async (int id, ProductDbContext db) =>
            {
                var product = await db.Products.FindAsync(id);
                if (product is null) return Results.NotFound();

                db.Products.Remove(product);
                await db.SaveChangesAsync();
                return Results.NoContent();
            })
            .WithName("DeleteProduct")
            .WithSummary("Elimina un producto");
        }
    }
}
