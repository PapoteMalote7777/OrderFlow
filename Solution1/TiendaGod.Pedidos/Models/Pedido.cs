using TiendaGod.Productos.Models;

namespace TiendaGod.Pedidos.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public decimal Total { get; set; }
        public OrderTypes Estado { get; set; } = OrderTypes.Pending;
        public DateTime EstadoActualizado { get; set; } = DateTime.UtcNow;
        public ICollection<PedidoProducto> PedidoProductos { get; set; }
    }
}
