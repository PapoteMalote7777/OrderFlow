using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaGod.Pedidos.Data;
using TiendaGod.Pedidos.DTO;
using TiendaGod.Productos.Models;
using TiendaGod.Shared.Events;

namespace TiendaGod.Pedidos.Controllers
{
    [ApiController]
    [Route("api/admin/pedidos")]
    [Authorize(Roles = "Admin")]
    public class PedidoAdminController : ControllerBase
    {
        private readonly PedidoDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        public PedidoAdminController(PedidoDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet("list")]
        public IActionResult GetAll()
        {
            var pedidos = _context.Pedidos
                .Include(p => p.PedidoProductos)
                .Select(p => new PedidoDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    Total = p.Total,
                    Estado = p.Estado.ToString(),
                    EstadoActualizado = p.EstadoActualizado,
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
                return NotFound();

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();

            return Ok("Pedido eliminado correctamente");
        }

        [HttpPost("{id}/accept")]
        public async Task<IActionResult> Accept(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return NotFound();

            if (pedido.Estado != OrderTypes.Pending)
                return BadRequest("Solo se pueden aceptar pedidos en estado Pending.");

            pedido.Estado = OrderTypes.Accepted;
            pedido.EstadoActualizado = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await _publishEndpoint.Publish(new OrderAcceptedEvent(
                PedidoId: pedido.Id,
                UserId: pedido.UserId,
                Total: pedido.Total
            ));
            return Ok("Pedido aceptado");
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return NotFound();

            if (pedido.Estado != OrderTypes.Pending)
                return BadRequest("Solo se pueden cancelar pedidos en estado Pending.");

            pedido.Estado = OrderTypes.Cancelled;
            pedido.EstadoActualizado = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await _publishEndpoint.Publish(new OrderRejectedEvent(
                PedidoId: pedido.Id,
                UserId: pedido.UserId,
                Reason: "Rechazado por el administrador"
            ));
            return Ok("Pedido cancelado");
        }
    }
}
