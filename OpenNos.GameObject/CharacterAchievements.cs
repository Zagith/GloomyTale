using OpenNos.Data.Achievements;
using OpenNos.GameObject.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.GameObject
{
    public class CharacterAchievements : CharacterAchievementDTO
    {
        #region Members

        private AchievementsDTO _quest;

        #endregion

        #region Instantiation

        public CharacterAchievements(CharacterAchievementDTO characterQuest)
        {
            CharacterId = characterQuest.CharacterId;
            AchievementId = characterQuest.AchievementId;
            FirstObjective = characterQuest.FirstObjective;
            IsMainAchievement = characterQuest.IsMainAchievement;
        }

        public CharacterAchievements(long questId, long characterId)
        {
            AchievementId = questId;
            CharacterId = characterId;
        }

        #endregion

        #region Properties

        public AchievementsDTO Achievements
        {
            get { return _quest ?? (_quest = ServerManager.Instance.GetAchievement(AchievementId)); }
        }

        #endregion
    }
}
