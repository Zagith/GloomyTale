using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Data
{
    public class UpgradeLogDTO
    {
        public long LogId { get; set; }

        public long CharacterId { get; set; }

        public Guid EquipmentSerialized { get; set; }

        public short Upgrade { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
