﻿using System;

namespace OpenNos.Data
{
    public class LogDropDTO
    {
        public long LogId { get; set; }

        public long? CharacterId { get; set; }

        public short ItemVNum { get; set; }

        public string ItemName { get; set; }

        public short Amount { get; set; }

        public short Map { get; set; }

        public short X { get; set; }

        public short Y { get; set; }

        public string IpAddress { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
