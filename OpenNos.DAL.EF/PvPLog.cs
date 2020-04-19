using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.EF
{
    public class PvPLog
    {
        [Key]
        public long LogId { get; set; }

        public virtual Character Character { get; set; }

        public long CharacterId { get; set; }

        public long TargetId { get; set; }

        [MaxLength(255)]
        public string IpAddress { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
