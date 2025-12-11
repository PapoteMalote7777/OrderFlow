using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiendaGod.Shared.Events
{
    public sealed record OrderRejectedEvent(
        int PedidoId,
        string UserId,
        string Reason
    ) : IRabbitEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}
