namespace TiendaGod.Productos.DTO
{
    public class ProductUpdateDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string Brand { get; set; } = null!;
        public int CategoryId { get; set; }
    }
}
