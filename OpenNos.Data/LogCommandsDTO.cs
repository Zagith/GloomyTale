using System;

namespace OpenNos.Data
{
    [Serializable]
    public class LogCommandsDTO
    {
        public long CommandId { get; set; }

        public long? CharacterId { get; set; }

        public string Command { get; set; }

        public string Data { get; set; }

        public string IpAddress { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
