using OpenNos.DAL.EF.Entities;
using OpenNos.Data.Achievements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Mapper.Mappers
{
    public static class CharacterAchievementMapper
    {
        #region Methods

        public static bool ToCharacterAchievement(CharacterAchievementDTO input, CharacterAchievement output)
        {
            if (input == null)
            {
                return false;
            }

            output.Id = input.Id;
            output.CharacterId = input.CharacterId;
            output.AchievementId = input.AchievementId;
            output.FirstObjective = input.FirstObjective;
            output.IsMainAchievement = input.IsMainAchievement;

            return true;
        }

        public static bool ToCharacterAchievementDTO(CharacterAchievement input, CharacterAchievementDTO output)
        {
            if (input == null)
            {
                return false;
            }

            output.Id = input.Id;
            output.CharacterId = input.CharacterId;
            output.AchievementId = input.AchievementId;
            output.FirstObjective = input.FirstObjective;
            output.IsMainAchievement = input.IsMainAchievement;

            return true;
        }

        #endregion
    }
}
