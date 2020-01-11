using System;
using System.Collections.Generic;
using System.Linq;
using GloomyTale.Core;
using GloomyTale.DAL.EF;
using GloomyTale.DAL.Interface;
using GloomyTale.Data;
using GloomyTale.DAL.EF.Helpers;

namespace GloomyTale.DAL.DAO
{
    public class QuestObjectiveDAO : IQuestObjectiveDAO
    {
        #region Methods

        public void Insert(List<QuestObjectiveDTO> quests)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    
                    foreach (QuestObjectiveDTO quest in quests)
                    {
                        QuestObjective entity = new QuestObjective();
                        Mapper.Mappers.QuestObjectiveMapper.ToQuestObjective(quest, entity);
                        context.QuestObjective.Add(entity);
                    }
                    
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        public QuestObjectiveDTO Insert(QuestObjectiveDTO questObjective)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    QuestObjective entity = new QuestObjective();
                    Mapper.Mappers.QuestObjectiveMapper.ToQuestObjective(questObjective, entity);
                    context.QuestObjective.Add(entity);
                    context.SaveChanges();
                    if (Mapper.Mappers.QuestObjectiveMapper.ToQuestObjectiveDTO(entity, questObjective))
                    {
                        return questObjective;
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

        public List<QuestObjectiveDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<QuestObjectiveDTO> result = new List<QuestObjectiveDTO>();
                foreach (QuestObjective questObjective in context.QuestObjective)
                {
                    QuestObjectiveDTO dto = new QuestObjectiveDTO();
                    Mapper.Mappers.QuestObjectiveMapper.ToQuestObjectiveDTO(questObjective, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public IEnumerable<QuestObjectiveDTO> LoadByQuestId(long questId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<QuestObjectiveDTO> result = new List<QuestObjectiveDTO>();
                foreach (QuestObjective questObjective in context.QuestObjective.Where(s => s.QuestId == questId))
                {
                    QuestObjectiveDTO dto = new QuestObjectiveDTO();
                    Mapper.Mappers.QuestObjectiveMapper.ToQuestObjectiveDTO(questObjective, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        #endregion
    }
}
