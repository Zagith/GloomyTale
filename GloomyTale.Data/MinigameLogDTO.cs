using System;
namespace GloomyTale.Data
{
    public class MinigameLogDTO : MappingBaseDTO
    {
        public long MinigameLogId { get; set; }

        public long StartTime { get; set; }

        public long EndTime { get; set; }

        public int Score { get; set; }

        public byte Minigame { get; set; }

        public long CharacterId { get; set; }
    }
}
