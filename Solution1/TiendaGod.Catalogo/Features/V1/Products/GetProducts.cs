using Microsoft.EntityFrameworkCore;
using TiendaGod.Productos.Data;
using TiendaGod.Productos.DTO;

namespace TiendaGod.Productos.Features.V1.Products
{
    public static class GetProducts
    {
        public static void MapGetProducts(this RouteGroupBuilder group)
        {
            group.MapGet("/", async (ProductDbContext db) =>
            {
                var products = await db.Products
                    .Include(p => p.Category)
                    .Select(p => new ProductDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        Description = p.Description,
                        Brand = p.Brand,
                        Stock = p.Stock,
                        CategoryId = p.CategoryId,
                        CategoryName = p.Category.Name
                    })
                    .ToListAsync();

                return Results.Ok(products);
            })
            .WithName("GetProducts")
            .WithSummary("Lista todos los productos")
            .WithOpenApi();
        }
    }
}
