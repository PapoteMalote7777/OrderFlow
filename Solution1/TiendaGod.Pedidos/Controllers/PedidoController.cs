using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaGod.Pedidos.Data;
using TiendaGod.Pedidos.DTO;
using TiendaGod.Pedidos.Models;

namespace TiendaGod.Pedidos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly PedidoDbContext _context;
        private readonly HttpClient _httpClient;

        public PedidosController(PedidoDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            var pedidos = _context.Pedidos.ToList();
            return Ok(pedidos);
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var pedido = _context.Pedidos.FirstOrDefault(p => p.Id == id);
            if (pedido == null) return NotFound();
            return Ok(pedido);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePedidoRequest request)
        {
            var userResp = await _httpClient.GetAsync($"https://api.tiendagod.identity/users/{request.UserId}");
            if (!userResp.IsSuccessStatusCode)
                return NotFound($"Usuario {request.UserId} no encontrado.");

            var pedido = new Pedido { UserId = request.UserId };
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            decimal totalPedido = 0;
            var pedidoProductos = new List<PedidoProducto>();

            foreach (var item in request.Productos)
            {
                var productResp = await _httpClient.GetAsync($"https://api.tiendagod.products/products/{item.ProductId}");
                if (!productResp.IsSuccessStatusCode)
                    return NotFound($"Producto {item.ProductId} no encontrado.");

                var productData = await productResp.Content.ReadFromJsonAsync<ProductDto>();

                decimal total = productData.Price * item.Cantidad;
                totalPedido += total;

                pedidoProductos.Add(new PedidoProducto
                {
                    ProductId = item.ProductId,
                    PrecioUnitario = productData.Price,
                    Cantidad = item.Cantidad,
                    Pedido = pedido
                });
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = pedido.Id }, new { pedido, productos = pedidoProductos });
        }
    }
}
