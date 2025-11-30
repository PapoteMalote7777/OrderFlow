using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaGod.Productos.Data;
using TiendaGod.Productos.Models;

namespace TiendaGod.Productos.Features.V1.Products
{
    public static class ProductEndpointsV1
    {
        public static RouteGroupBuilder MapProductEndpointsV1(this WebApplication app)
        {
            var group = app.MapGroup("/api/v1/products")
                           .WithTags("Products v1")
                           .WithOpenApi();

            group.MapGetProducts();
            group.MapGetProductById();
            group.MapCreateProduct().RequireAuthorization();
            group.MapUpdateProduct().RequireAuthorization();
            group.MapDeleteProduct().RequireAuthorization();

            return group;
        }
    }
}
