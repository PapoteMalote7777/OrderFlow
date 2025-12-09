using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaGod.Pedidos.Data;
using TiendaGod.Pedidos.DTO;

namespace TiendaGod.Pedidos.Controllers
{
    [ApiController]
    [Route("api/admin/pedidos")]
    [Authorize(Roles = "Admin")]
    public class PedidoAdminController : ControllerBase
    {
        private readonly PedidoDbContext _context;

        public PedidoAdminController(PedidoDbContext context)
        {
            _context = context;
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
    }
}
