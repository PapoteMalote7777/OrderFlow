namespace TiendaGod.Pedidos.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ICollection<PedidoProducto> PedidoProductos { get; set; } = new List<PedidoProducto>();
    }
}
