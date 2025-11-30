using TiendaGod.Productos.Data;

namespace TiendaGod.Productos.Features.V1.Products
{
    public static class DeleteProduct
    {
        public static RouteHandlerBuilder MapDeleteProduct(this RouteGroupBuilder group)
        {
            return group.MapDelete("/{id:int}", async (int id, ProductDbContext db) =>
            {
                var product = await db.Products.FindAsync(id);
                if (product is null) return Results.NotFound();

                db.Products.Remove(product);
                await db.SaveChangesAsync();
                return Results.NoContent();
            })
            .WithName("DeleteProduct")
            .WithSummary("Elimina un producto")
            .WithOpenApi();
        }
    }
}
