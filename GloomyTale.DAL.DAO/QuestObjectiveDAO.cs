using System;
using System.Collections.Generic;
using System.Linq;
using GloomyTale.Core;
using GloomyTale.DAL.EF;
using GloomyTale.DAL.Interface;
using GloomyTale.Data;
using GloomyTale.DAL.EF.Helpers;
using AutoMapper;

namespace GloomyTale.DAL.DAO
{
    public class QuestObjectiveDAO : MappingBaseDao<QuestObjective, QuestObjectiveDTO>, IQuestObjectiveDAO
    {
        public QuestObjectiveDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public void Insert(List<QuestObjectiveDTO> quests)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {

                    foreach (QuestObjectiveDTO quest in quests)
                    {
                        var entity = _mapper.Map<QuestObjective>(quest);
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
                    var entity = _mapper.Map<QuestObjective>(questObjective);
                    context.QuestObjective.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<QuestObjectiveDTO>(questObjective);
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
                return context.QuestObjective.ToList().Select(d => _mapper.Map<QuestObjectiveDTO>(d)).ToList();
            }
        }

        public IEnumerable<QuestObjectiveDTO> LoadByQuestId(long questId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (QuestObjective reward in context.QuestObjective.Where(s => s.QuestId == questId))
                {
                    yield return _mapper.Map<QuestObjectiveDTO>(reward);
                }
            }
        }

        #endregion
    }
}
