using System;

namespace OpenNos.Data.Achievements
{
    [Serializable]
    public class CharacterAchievementDTO : SynchronizableBaseDTO
    {
        #region Properties

        public long CharacterId { get; set; }

        public long AchievementId { get; set; }

        public int FirstObjective { get; set; }

        public bool IsMainAchievement { get; set; }

        #endregion      
    }
}
