using System;

namespace GloomyTale.Data
{
    [Serializable]
    public class TimeSpacesLogDTO : MappingBaseDTO
    {
        public long LogId { get; set; }

        public long? CharacterId { get; set; }

        public long TimeSpaceId { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
