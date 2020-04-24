using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.EF.Entities
{
    public class UpgradeLog
    {
        [Key]
        public long LogId { get; set; }

        public long CharacterId { get; set; }

        public Guid EquipmentSerialized { get; set; }

        public short Upgrade { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
