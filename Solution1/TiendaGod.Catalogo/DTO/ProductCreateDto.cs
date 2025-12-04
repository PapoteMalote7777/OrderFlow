namespace TiendaGod.Productos.DTO
{
    public class ProductCreateDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string Brand { get; set; } = null!;
        public int Stock { get; set; }
        public int CategoryId { get; set; }
    }
}
