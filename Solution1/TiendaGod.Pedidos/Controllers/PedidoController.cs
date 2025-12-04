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
        [FromServices] ProductsHttpService productosService,
        [FromServices] IdentityHttpService identityService)
        {
            // ✅ 1. Validar Usuario
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var userExiste = await identityService.UserExiste(request.UserId, token);
            if (!userExiste)
                return NotFound($"Usuario {request.UserId} no encontrado.");

            // ✅ 2. Crear pedido vacío
            var pedido = new Pedido
            {
                UserId = request.UserId,
                Total = 0
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            decimal totalPedido = 0;
            var pedidoProductos = new List<PedidoProducto>();

            // ✅ 3. Validar productos uno por uno
            foreach (var item in request.Productos)
            {
                var producto = await productosService.GetProducto(item.ProductId);

                if (producto == null)
                    return NotFound($"Producto {item.ProductId} no existe.");

                if (producto.Stock < item.Cantidad)
                    return BadRequest($"Stock insuficiente para {producto.Name}");

                var totalProducto = producto.Price * item.Cantidad;
                totalPedido += totalProducto;

                pedidoProductos.Add(new PedidoProducto
                {
                    PedidoId = pedido.Id,
                    ProductId = producto.Id,
                    NombreProducto = producto.Name,
                    PrecioUnitario = producto.Price,
                    Cantidad = item.Cantidad
                });

                // ✅ 4. Descontar stock en API Productos
                var stockDescontado = await productosService
                    .DescontarStock(producto.Id, item.Cantidad);

                if (!stockDescontado)
                    return StatusCode(500, "Error al descontar stock");
            }

            // ✅ 5. Guardar productos del pedido
            _context.PedidoProducto.AddRange(pedidoProductos);

            // ✅ 6. Guardar total
            pedido.Total = totalPedido;

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById),
                new { id = pedido.Id },
                new
                {
                    pedido.Id,
                    pedido.UserId,
                    Total = pedido.Total,
                    Productos = pedidoProductos
                });
        }
    }
}
