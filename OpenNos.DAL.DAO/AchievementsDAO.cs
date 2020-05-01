using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Entities;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using OpenNos.Data.Achievements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.DAO
{
    public class AchievementsDAO : IAchievementsDAO
    {
        #region Methods

        public AchievementsDTO LoadById(long accountId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Achievement account = context.Achievement.FirstOrDefault(a => a.AchievementId.Equals(accountId));
                    if (account != null)
                    {
                        AchievementsDTO accountDTO = new AchievementsDTO();
                        if (Mapper.Mappers.AchievementMapper.ToAchievementDTO(account, accountDTO))
                        {
                            return accountDTO;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return null;
        }

        public IEnumerable<AchievementsDTO> LoadByType(int type)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<AchievementsDTO> result = new List<AchievementsDTO>();
                foreach (Achievement itemInstance in context.Achievement.Where(i => i.AchievementType == type))
                {
                    AchievementsDTO output = new AchievementsDTO();
                    Mapper.Mappers.AchievementMapper.ToAchievementDTO(itemInstance, output);
                    result.Add(output);
                }
                return result;
            }
        }

        public AchievementsDTO LoadByData2(int accountId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Achievement account = context.Achievement.FirstOrDefault(a => a.Data2.Equals(accountId));
                    if (account != null)
                    {
                        AchievementsDTO accountDTO = new AchievementsDTO();
                        if (Mapper.Mappers.AchievementMapper.ToAchievementDTO(account, accountDTO))
                        {
                            return accountDTO;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return null;
        }
        #endregion
    }
}
