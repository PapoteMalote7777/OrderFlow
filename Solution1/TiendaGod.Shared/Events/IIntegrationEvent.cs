using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiendaGod.Shared.Events
{
    public interface IRabbitEvent
    {
       public Guid EventId { get; }
       public DateTime Timestamp { get; }
    }
}
