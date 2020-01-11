using GloomyTale.Core;
using GloomyTale.DAL.EF;
using GloomyTale.DAL.EF.Helpers;
using GloomyTale.DAL.Interface;
using GloomyTale.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GloomyTale.DAL.DAO
{
    public class QuestRewardDAO : IQuestRewardDAO
    {
        #region Methods

        public void Insert(List<QuestRewardDTO> questRewards)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    
                    foreach (QuestRewardDTO rewards in questRewards)
                    {
                        QuestReward entity = new QuestReward();
                        Mapper.Mappers.QuestRewardMapper.ToQuestReward(rewards, entity);
                        context.QuestReward.Add(entity);
                    }
                    
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        public QuestRewardDTO Insert(QuestRewardDTO questReward)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    QuestReward entity = new QuestReward();
                    Mapper.Mappers.QuestRewardMapper.ToQuestReward(questReward, entity);
                    context.QuestReward.Add(entity);
                    context.SaveChanges();
                    if (Mapper.Mappers.QuestRewardMapper.ToQuestRewardDTO(entity, questReward))
                    {
                        return questReward;
                    }

                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public List<QuestRewardDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<QuestRewardDTO> result = new List<QuestRewardDTO>();
                foreach (QuestReward entity in context.QuestReward)
                {
                    QuestRewardDTO dto = new QuestRewardDTO();
                    Mapper.Mappers.QuestRewardMapper.ToQuestRewardDTO(entity, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public IEnumerable<QuestRewardDTO> LoadByQuestId(long questId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<QuestRewardDTO> result = new List<QuestRewardDTO>();
                foreach (QuestReward reward in context.QuestReward.Where(s => s.QuestId == questId))
                {
                    QuestRewardDTO dto = new QuestRewardDTO();
                    Mapper.Mappers.QuestRewardMapper.ToQuestRewardDTO(reward, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        #endregion
    }
}

