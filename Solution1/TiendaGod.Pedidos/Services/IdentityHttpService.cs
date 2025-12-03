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

        public async Task<bool> ProductoExiste(int UserId)
        {
            var response = await _http.GetAsync($"/api/user/{UserId}");
            return response.StatusCode == HttpStatusCode.OK;
        }
    }
}
