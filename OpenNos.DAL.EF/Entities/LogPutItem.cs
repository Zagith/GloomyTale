using System;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.DAL.EF.Entities
{
    public class LogPutItem
    {
        [Key]
        public long LogId { get; set; }

        public long? CharacterId { get; set; }

        public short ItemVNum { get; set; }

        public short Amount { get; set; }

        public short Map { get; set; }

        public short X { get; set; }

        public short Y { get; set; }

        [MaxLength(255)]
        public string IpAddress { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
