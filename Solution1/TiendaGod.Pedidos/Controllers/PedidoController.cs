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
            var pedidos = _context.Pedidos
                .Include(p => p.PedidoProductos)
                .ToList()
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
                });

            return Ok(pedidos);
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var pedido = _context.Pedidos
                .Include(p => p.PedidoProductos)
                .FirstOrDefault(p => p.Id == id);

            if (pedido == null) return NotFound();

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

            return Ok(pedidoDto);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(
        [FromBody] CreatePedidoRequest request,
        [FromServices] ProductsHttpService productosService,
        [FromServices] IdentityHttpService identityService)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            Console.WriteLine("TOKEN RECIBIDO:");
            Console.WriteLine(token);

            var userExiste = await identityService.UserExiste(request.UserId, token);
            if (!userExiste)
                return NotFound($"Usuario {request.UserId} no encontrado.");

            var pedido = new Pedido
            {
                UserId = request.UserId,
                Total = 0,
                PedidoProductos = new List<PedidoProducto>()
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            decimal totalPedido = 0;

            foreach (var item in request.Productos)
            {
                var producto = await productosService.GetProducto(item.ProductId, token);

                if (producto == null)
                    return NotFound($"Producto {item.ProductId} no existe.");

                if (producto.Stock < item.Cantidad)
                    return BadRequest($"Stock insuficiente para {producto.Name}");

                var totalProducto = producto.Price * item.Cantidad;
                totalPedido += totalProducto;

                var pedidoProducto = new PedidoProducto
                {
                    PedidoId = pedido.Id,
                    ProductId = producto.Id,
                    NombreProducto = producto.Name,
                    PrecioUnitario = producto.Price,
                    Cantidad = item.Cantidad
                };
                pedido.PedidoProductos.Add(pedidoProducto);
                var stockDescontado = await productosService.DescontarStock(producto.Id, item.Cantidad, token);

                if (!stockDescontado)
                    return StatusCode(500, "Error al descontar stock");
            }

            pedido.Total = totalPedido;
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
            return CreatedAtAction(nameof(GetById), new { id = pedido.Id }, pedidoDto);
        }
    }
}
