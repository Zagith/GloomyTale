using System;
using System.ComponentModel.DataAnnotations;

namespace GloomyTale.DAL.EF.Entities
{
    public class TimeSpacesLog
    {
        [Key]
        public long LogId { get; set; }

        public long? CharacterId { get; set; }

        public long TimeSpaceId { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
