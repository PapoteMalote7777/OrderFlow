namespace TiendaGod.Pedidos.DTO
{
    public class PedidoDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public decimal Total { get; set; }
        public List<PedidoProductoDto> Productos { get; set; }
    }
}
