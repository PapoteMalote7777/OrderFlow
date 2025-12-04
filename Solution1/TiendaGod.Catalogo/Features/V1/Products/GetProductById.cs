using Microsoft.EntityFrameworkCore;
using TiendaGod.Productos.Data;
using TiendaGod.Productos.DTO;

namespace TiendaGod.Productos.Features.V1.Products
{
    public static class GetProductById
    {
        public static void MapGetProductById(this RouteGroupBuilder group)
        {
            group.MapGet("/{id:int}", async (int id, ProductDbContext db) =>
            {
                var product = await db.Products
                    .Include(p => p.Category)
                    .Where(p => p.Id == id)
                    .Select(p => new ProductDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        Description = p.Description,
                        Brand = p.Brand,
                        Stock = p.Stock,
                        CategoryId = p.CategoryId,
                        CategoryName = p.Category!.Name
                    })
                    .FirstOrDefaultAsync();

                return product is null ? Results.NotFound() : Results.Ok(product);
            })
            .WithName("GetProductById")
            .WithSummary("Obtiene un producto por ID")
            .WithOpenApi();
        }
    }
}
