﻿using System;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.DAL.EF
{
    public class LogDrop
    {
        [Key]
        public long LogId { get; set; }

        public virtual Character Character { get; set; }

        public long? CharacterId { get; set; }

        public short ItemVNum { get; set; }

        [MaxLength(255)]
        public string ItemName { get; set; }

        public short Amount { get; set; }

        public short Map { get; set; }

        public short X { get; set; }

        public short Y { get; set; }

        [MaxLength(255)]
        public string IpAddress { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
