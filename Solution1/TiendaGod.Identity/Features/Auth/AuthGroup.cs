namespace TiendaGod.Identity.Features.Auth
{
    public static class AuthGroup
    {
        public static RouteGroupBuilder MapAuthGroup(this IEndpointRouteBuilder routes)
        {
            var versionSet = routes.NewApiVersionSet()
                .HasApiVersion(new Asp.Versioning.ApiVersion(1, 0))
                .Build();
            var group = routes.MapGroup("/api/v{version.apiVersion}/auth");
            group.WithApiVersionSet(versionSet);
            group.WithTags("Auth");
            return group;
        }
    }
}
