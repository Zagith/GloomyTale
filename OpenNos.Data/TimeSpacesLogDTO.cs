﻿using System;

namespace OpenNos.Data
{
    [Serializable]
    public class TimeSpacesLogDTO
    {
        public long LogId { get; set; }

        public long? CharacterId { get; set; }

        public long TimeSpaceId { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
