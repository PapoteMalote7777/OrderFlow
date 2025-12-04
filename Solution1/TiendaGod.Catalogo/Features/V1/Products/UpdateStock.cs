using Microsoft.EntityFrameworkCore;
using TiendaGod.Productos.Data;

namespace TiendaGod.Productos.Features.V1.Products
{
    public static class UpdateStock
    {
        public static RouteHandlerBuilder MapPutStock(this RouteGroupBuilder group)
        {
            return group.MapPut("/{id}/stock/{cantidad}", async (
                int id,
                int cantidad,
                ProductDbContext db) =>
            {
                var producto = await db.Products.FirstOrDefaultAsync(p => p.Id == id);

                if (producto == null)
                    return Results.NotFound("Producto no existe");

                if (producto.Stock < cantidad)
                    return Results.BadRequest("Stock insuficiente");

                producto.Stock -= cantidad;
                await db.SaveChangesAsync();

                return Results.Ok(producto.Stock);
            });
        }
    }
}
