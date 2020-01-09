using OpenNos.Domain;
using System;

namespace OpenNos.Data
{
    [Serializable]
    public class LogChatDTO
    {
        public long LogId { get; set; }

        public long? CharacterId { get; set; }

        public ChatType ChatType { get; set; }

        public string ChatMessage { get; set; }

        public string IpAddress { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
