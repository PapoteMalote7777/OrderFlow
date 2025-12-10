using System.Net;
using System.Net.Http.Headers;
using TiendaGod.Pedidos.DTO;

namespace TiendaGod.Pedidos.Services
{
    public class ProductsHttpService
    {
        private readonly HttpClient _http;

        public ProductsHttpService(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("tiendagod-productos");
        }

        public async Task<ProductDto?> GetProducto(int productId, string token)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/api/v1/products/{productId}");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<ProductDto>();
        }

        public async Task<bool> DescontarStock(int productId, int cantidad, string token)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"/api/v1/products/{productId}/stock/{cantidad}");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(request);

            return response.IsSuccessStatusCode;
        }
    }
}