namespace OpenNos.DAL.EF.Entities
{
    public class CharacterAchievement : SynchronizableBaseEntity
    {
        #region Properties

        public long CharacterId { get; set; }

        public virtual Achievement Achievement { get; set; }

        public long AchievementId { get; set; }

        public int FirstObjective { get; set; }

        public bool IsMainAchievement { get; set; }

        #endregion        
    }
}
