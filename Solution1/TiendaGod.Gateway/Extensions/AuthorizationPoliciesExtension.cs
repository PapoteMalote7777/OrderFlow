namespace TiendaGod.Gateway.Extensions
{
    public static class AuthorizationPoliciesExtensions
    {
        public static IServiceCollection AddGatewayAuthorizationPolicies(
            this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("authenticated", policy =>
                    policy.RequireAuthenticatedUser());

                options.AddPolicy("admin", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Admin");
                });

                options.AddPolicy("user", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("User");
                });

                options.FallbackPolicy = null;
            });

            return services;
        }
    }
}
