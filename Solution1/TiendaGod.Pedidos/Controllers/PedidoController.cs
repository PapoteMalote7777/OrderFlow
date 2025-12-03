using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaGod.Pedidos.Data;
using TiendaGod.Pedidos.DTO;
using TiendaGod.Pedidos.Models;
using TiendaGod.Pedidos.Services;

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
        public async Task<IActionResult> Create(
            [FromBody] CreatePedidoRequest request,
            [FromServices] ProductsHttpService productosService)
        {
            var userResp = await _httpClient.GetAsync($"https://api.tiendagod.identity/users/{request.UserId}");
            if (!userResp.IsSuccessStatusCode)
                return NotFound($"Usuario {request.UserId} no encontrado.");

            var productosValidados = new List<(int ProductId, decimal Precio, int Cantidad)>();

            foreach (var item in request.Productos)
            {
                var producto = await productosService.GetProducto(item.ProductId);

                if (producto == null)
                    return NotFound($"Producto {item.ProductId} no encontrado.");

                productosValidados.Add((
                    item.ProductId,
                    producto.Price,
                    item.Cantidad
                ));
            }

            var pedido = new Pedido { UserId = request.UserId };
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            decimal totalPedido = 0;
            var pedidoProductos = new List<PedidoProducto>();

            foreach (var item in productosValidados)
            {
                decimal total = item.Precio * item.Cantidad;
                totalPedido += total;

                pedidoProductos.Add(new PedidoProducto
                {
                    PedidoId = pedido.Id,
                    ProductId = item.ProductId,
                    PrecioUnitario = item.Precio,
                    Cantidad = item.Cantidad
                });
            }

            _context.PedidoProducto.AddRange(pedidoProductos);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById),
                new { id = pedido.Id },
                new { pedido, productos = pedidoProductos, total = totalPedido });
        }
    }
}
