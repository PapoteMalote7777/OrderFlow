using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiendaGod.Shared.Events
{
    public sealed record OrderAcceptedEvent(
        int PedidoId,
        string UserId,
        decimal Total
    ) : IRabbitEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}
