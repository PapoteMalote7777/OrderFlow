using System.Net;

namespace TiendaGod.Pedidos.Services
{
    public class IdentityHttpService
    {
        private readonly HttpClient _http;

        public IdentityHttpService(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("TiendaGod-Identity");
        }

        // Método que usa el JWT del usuario
        public async Task<bool> UserExiste(string userId, string jwtToken)
        {
            // Añadir el header Authorization
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _http.GetAsync($"/api/User/exists/{userId}");
            return response.IsSuccessStatusCode;
        }
    }
}
