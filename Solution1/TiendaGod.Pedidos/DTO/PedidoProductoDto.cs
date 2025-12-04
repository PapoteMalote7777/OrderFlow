namespace TiendaGod.Pedidos.DTO
{
    public class PedidoProductoDto
    {
        public int ProductId { get; set; }
        public string NombreProducto { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
    }
}
