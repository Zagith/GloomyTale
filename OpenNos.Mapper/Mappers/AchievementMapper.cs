using OpenNos.DAL.EF.Entities;
using OpenNos.Data.Achievements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Mapper.Mappers
{
    public static class AchievementMapper
    {
        #region Methods

        public static bool ToAchievement(AchievementsDTO input, Achievement output)
        {
            if (input == null)
            {
                return false;
            }

            output.AchievementId = input.AchievementId;
            output.Name = input.Name;
            output.AchievementType = input.AchievementType;
            output.Name = input.Name;
            output.Data = input.Data;
            output.LevelMin = input.LevelMin;
            output.LevelMax = input.LevelMax;
            output.IsDaily = input.IsDaily;
            output.Data2 = input.Data2;
            output.Category = input.Category;

            return true;
        }

        public static bool ToAchievementDTO(Achievement input, AchievementsDTO output)
        {
            if (input == null)
            {
                return false;
            }

            output.AchievementId = input.AchievementId;
            output.Name = input.Name;
            output.AchievementType = input.AchievementType;
            output.Name = input.Name;
            output.Data = input.Data;
            output.LevelMin = input.LevelMin;
            output.LevelMax = input.LevelMax;
            output.IsDaily = input.IsDaily;
            output.Data2 = input.Data2;
            output.Category = input.Category;

            return true;
        }

        #endregion
    }
}
