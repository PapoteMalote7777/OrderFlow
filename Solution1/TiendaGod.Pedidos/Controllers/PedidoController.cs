using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TiendaGod.Pedidos.Data;
using TiendaGod.Pedidos.DTO;
using TiendaGod.Pedidos.Models;
using TiendaGod.Pedidos.Services;

namespace TiendaGod.Pedidos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PedidosController : ControllerBase
    {
        private readonly PedidoDbContext _context;
        private readonly HttpClient _httpClient;

        public PedidosController(PedidoDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet("list")]
        public IActionResult GetMyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var pedidos = _context.Pedidos
                .Where(p => p.UserId == userId)
                .Include(p => p.PedidoProductos)
                .Select(p => new PedidoDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    Total = p.Total,
                    Productos = p.PedidoProductos.Select(pp => new PedidoProductoDto
                    {
                        ProductId = pp.ProductId,
                        NombreProducto = pp.NombreProducto,
                        PrecioUnitario = pp.PrecioUnitario,
                        Cantidad = pp.Cantidad
                    }).ToList()
                })
                .ToList();

            return Ok(pedidos);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(
        [FromBody] CreatePedidoRequest request,
        [FromServices] ProductsHttpService productosService)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            foreach (var item in request.Productos)
            {
                var producto = await productosService.GetProducto(item.ProductId, HttpContext.Request.Headers["Authorization"]);
                if (producto == null) return NotFound($"Producto {item.ProductId} no existe.");
                if (producto.Stock < item.Cantidad) return BadRequest($"Stock insuficiente para {producto.Name}");
            }

            var pedido = new Pedido
            {
                UserId = userId,
                Total = 0,
                PedidoProductos = new List<PedidoProducto>()
            };

            decimal totalPedido = 0;

            foreach (var item in request.Productos)
            {
                var producto = await productosService.GetProducto(item.ProductId, HttpContext.Request.Headers["Authorization"]);

                var totalProducto = producto.Price * item.Cantidad;
                totalPedido += totalProducto;

                var pedidoProducto = new PedidoProducto
                {
                    ProductId = producto.Id,
                    NombreProducto = producto.Name,
                    PrecioUnitario = producto.Price,
                    Cantidad = item.Cantidad
                };
                pedido.PedidoProductos.Add(pedidoProducto);

                var stockDescontado = await productosService.DescontarStock(producto.Id, item.Cantidad, HttpContext.Request.Headers["Authorization"]);
                if (!stockDescontado)
                    return StatusCode(500, "Error al descontar stock");
            }

            pedido.Total = totalPedido;
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            var pedidoDto = new PedidoDto
            {
                Id = pedido.Id,
                UserId = pedido.UserId,
                Total = pedido.Total,
                Productos = pedido.PedidoProductos.Select(pp => new PedidoProductoDto
                {
                    ProductId = pp.ProductId,
                    NombreProducto = pp.NombreProducto,
                    PrecioUnitario = pp.PrecioUnitario,
                    Cantidad = pp.Cantidad
                }).ToList()
            };

            return CreatedAtAction(nameof(GetMyOrders), new { id = pedido.Id }, pedidoDto);
        }
    }
}
