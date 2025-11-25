using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaGod.Productos.Data;
using TiendaGod.Productos.Models;

namespace TiendaGod.Productos.Features.V1
{
    public static class ProductEndpointsV1
    {
        public static RouteGroupBuilder MapProductEndpointsV1(this WebApplication app)
        {
            var group = app.MapGroup("/api/v1/products")
                           .WithTags("Products v1")
                           .WithOpenApi();

            // Registrar endpoints
            group.MapGetProducts();
            group.MapGetProductById();
            group.MapCreateProduct();
            group.MapUpdateProduct();
            group.MapDeleteProduct();

            return group;
        }
    }
}
