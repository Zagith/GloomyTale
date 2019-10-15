using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.EF
{
    public class LogPutItem
    {
        [Key]
        public long LogId { get; set; }

        public long? CharacterId { get; set; }

        public short ItemVNum { get; set; }

        public short Amount { get; set; }

        public short Map { get; set; }

        public byte X { get; set; }

        public byte Y { get; set; }

        [MaxLength(255)]
        public string IpAddress { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
