using TiendaGod.Productos.Data;
using TiendaGod.Productos.Models;

namespace TiendaGod.Productos.DTO
{
    public static class CategorySeeder
    {
        public static void Seed(ProductDbContext db)
        {
            if (!db.Categories.Any())
            {
                db.Categories.AddRange(
                    new Category { Name = "Deportes" },
                    new Category { Name = "Ropa" },
                    new Category { Name = "Juguetes" },
                    new Category { Name = "Electrónica" },
                    new Category { Name = "Comida" },
                    new Category { Name = "Muebles" }
                );

                db.SaveChanges();
            }
        }
    }
}
