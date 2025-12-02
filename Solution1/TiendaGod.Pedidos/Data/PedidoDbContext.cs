using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TiendaGod.Pedidos.Models;

namespace TiendaGod.Pedidos.Data
{
    public class PedidoDbContext : DbContext
    {
        public PedidoDbContext(DbContextOptions<PedidoDbContext> options)
            : base(options)
        {
        }

        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoProducto> PedidoProducto { get; set; }
    }
}
