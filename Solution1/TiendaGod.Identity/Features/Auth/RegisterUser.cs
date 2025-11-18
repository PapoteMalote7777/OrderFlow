namespace TiendaGod.Identity.Features.Auth
{
    public static class RegisterUser
    {
        public record Request(string Name, string Email, string Password);
        public record Response(string UserId, string UserName, string Email);
         // falta seguir por aqui
        /*public static IEndpointRouteBuilder MapRegisterUser(this IEndpointRouteBuilder group)
        {
            var AuthGroup = group.MapAuthGroup();
            AuthGroup.MapPost("/register", HandlerAsync)
        }*/
    }
}
