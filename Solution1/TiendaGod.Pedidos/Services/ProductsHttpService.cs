using System.Net;
using TiendaGod.Pedidos.DTO;

namespace TiendaGod.Pedidos.Services
{
    public class ProductsHttpService
    {
        private readonly HttpClient _http;

        public ProductsHttpService(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("TiendaGod-Productos");
        }

        public async Task<ProductDto?> GetProducto(int productId)
        {
            var response = await _http.GetAsync($"/api/v1/products/{productId}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<ProductDto>();
        }
    }
}