namespace TiendaGod.Productos.Features.V1.Categories
{
    public static class CategoryEndpointsV1
    {
        public static RouteGroupBuilder MapCategoryEndpointsV1(this WebApplication app)
        {
            var group = app.MapGroup("/api/v1/categories")
                           .WithTags("Categories V1")
                           .WithOpenApi();

            group.MapGetCategories();
            group.MapCreateCategory().RequireAuthorization();
            group.MapDeleteCategory().RequireAuthorization();

            return group;
        }
    }
}
