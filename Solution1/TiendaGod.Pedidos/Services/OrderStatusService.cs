using Microsoft.EntityFrameworkCore;
using TiendaGod.Pedidos.Data;
using TiendaGod.Productos.Models;

public class OrderStatusService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public OrderStatusService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<PedidoDbContext>();

                var pedidos = await context.Pedidos
                    .Where(p => p.Estado != OrderTypes.Delivered &&
                                p.Estado != OrderTypes.Cancelled)
                    .ToListAsync(stoppingToken);

                foreach (var p in pedidos)
                {
                    if ((DateTime.UtcNow - p.EstadoActualizado).TotalMinutes >= 5)
                    {
                        p.Estado = NextState(p.Estado);
                        p.EstadoActualizado = DateTime.UtcNow;
                    }
                }

                await context.SaveChangesAsync(stoppingToken);
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private OrderTypes NextState(OrderTypes current)
    {
        return current switch
        {
            OrderTypes.Accepted => OrderTypes.Shipped,
            OrderTypes.Shipped => OrderTypes.Ontheway,
            OrderTypes.Ontheway => OrderTypes.Delivered,
            _ => current
        };
    }
}
