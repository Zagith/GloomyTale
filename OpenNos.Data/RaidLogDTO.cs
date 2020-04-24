using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Data
{
    public class RaidLogDTO
    {
        public long LogId { get; set; }

        public long CharacterId { get; set; }

        public long RaidId { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
