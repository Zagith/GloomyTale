using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Data
{
    [Serializable]
    public class PvPLogDTO
    {
        public long LogId { get; set; }

        public long CharacterId { get; set; }

        public long TargetId { get; set; }

        public string IpAddress { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
