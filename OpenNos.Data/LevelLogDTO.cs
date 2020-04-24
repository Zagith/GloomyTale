using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Data
{
    public class LevelLogDTO
    {
        public long LogId { get; set; }

        public long CharacterId { get; set; }

        public byte Level { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
