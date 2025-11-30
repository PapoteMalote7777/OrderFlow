using Microsoft.AspNetCore.Mvc;
using TiendaGod.Productos.Data;
using TiendaGod.Productos.DTO;
using TiendaGod.Productos.Models;

namespace TiendaGod.Productos.Features.V1.Products
{
    public static class CreateProduct
    {
        public static RouteHandlerBuilder MapCreateProduct(this RouteGroupBuilder group)
        {
            return group.MapPost("/", async ([FromBody] ProductCreateDto dto, ProductDbContext db) =>
            {
                var product = new Product
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Price = dto.Price,
                    Brand = dto.Brand,
                    CategoryId = dto.CategoryId
                };

                db.Products.Add(product);
                await db.SaveChangesAsync();

                return Results.Created($"/api/v1/products/{product.Id}", product);
            })
            .WithName("CreateProduct")
            .WithSummary("Crea un nuevo producto")
            .WithOpenApi();
        }
    }

}
