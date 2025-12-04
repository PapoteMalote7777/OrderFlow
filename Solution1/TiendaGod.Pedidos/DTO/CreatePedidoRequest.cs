namespace TiendaGod.Pedidos.DTO
{
    public class CreatePedidoRequest
    {
        public string UserId { get; set; }
        public List<CreatePedidoProductoDto> Productos { get; set; }
    }
}
