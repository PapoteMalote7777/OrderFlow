namespace TiendaGod.Pedidos.DTO
{
    public class CreatePedidoRequest
    {
        public string UserId { get; set; }
        public List<ProductItem> Productos { get; set; }
    }
}
