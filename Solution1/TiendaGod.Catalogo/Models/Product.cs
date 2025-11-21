namespace TiendaGod.Productos.Models
{
    public class Product
    {
        public int Id { get; set; }                      // PK
        public string Name { get; set; } = string.Empty; // nombre
        public string? Description { get; set; }         // descripción (opcional)
        public decimal Price { get; set; }               // precio
        public string? Brand { get; set; }               // marca (opcional)
    }
}
