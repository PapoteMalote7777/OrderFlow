using Microsoft.AspNetCore.Mvc;
using TiendaGod.Productos.Data;
using TiendaGod.Productos.Models;

namespace TiendaGod.Productos.Features.V1.Categories
{
    public static class CreateCategory
    {
        public static RouteHandlerBuilder MapCreateCategory(this RouteGroupBuilder group)
        {
            return group.MapPost("/", async ([FromBody] Category category, ProductDbContext db) =>
            {
                db.Categories.Add(category);
                await db.SaveChangesAsync();
                return Results.Created($"/api/v1/categories/{category.Id}", category);
            })
            .WithName("CreateCategory")
            .WithSummary("Crea una nueva categoría")
            .WithOpenApi();
        }
    }
}
