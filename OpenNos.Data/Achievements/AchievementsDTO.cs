
using System;

namespace OpenNos.Data.Achievements
{
    [Serializable]
    public class AchievementsDTO
    {
        #region Properties

        public long AchievementId { get; set; }

        public string Name { get; set; }

        public int AchievementType { get; set; }

        public int Data { get; set; }

        public byte LevelMin { get; set; }

        public byte LevelMax { get; set; }

        public bool IsDaily { get; set; }

        public byte Category { get; set; }

        public int Data2 { get; set; }

        #endregion
    }
}
