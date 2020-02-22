using System;
using System.Collections.Generic;
using System.Linq;
using GloomyTale.Core;
using GloomyTale.DAL.EF;
using GloomyTale.DAL.Interface;
using GloomyTale.Data;
using GloomyTale.Data.Enums;
using GloomyTale.DAL.EF.Helpers;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace GloomyTale.DAL.DAO
{
    public class QuestLogDAO : MappingBaseDao<QuestLog, QuestLogDTO>, IQuestLogDAO
    {
        public QuestLogDAO(IMapper mapper) : base(mapper)
        { }

        public SaveResult InsertOrUpdate(ref QuestLogDTO quest)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long questId = quest.QuestId;
                    long characterId = quest.CharacterId;
                    QuestLog entity = context.QuestLog.FirstOrDefault(c => c.QuestId.Equals(questId) && c.CharacterId.Equals(characterId));

                    if (entity == null)
                    {
                        quest = Insert(quest, context);
                        return SaveResult.Inserted;
                    }

                    quest.QuestId = entity.QuestId;
                    quest = Update(entity, quest, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return SaveResult.Error;
            }
        }

        public QuestLogDTO Insert(QuestLogDTO questLog, OpenNosContext context)
        {
            try
            {
                var entity = _mapper.Map<QuestLog>(questLog);
                context.QuestLog.Add(entity);
                context.SaveChanges();
                return _mapper.Map<QuestLogDTO>(entity);
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public QuestLogDTO Update(QuestLog old, QuestLogDTO replace, OpenNosContext context)
        {
            if (old != null)
            {
                _mapper.Map(old, replace);
                context.SaveChanges();
            }

            return _mapper.Map<QuestLogDTO>(old);
        }

        public QuestLogDTO LoadById(long id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<QuestLogDTO>(context.QuestLog.FirstOrDefault(i => i.QuestId == id));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<QuestLogDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (QuestLog id in context.QuestLog.Where(c => c.CharacterId == characterId))
                {
                    yield return _mapper.Map<QuestLogDTO>(id);
                }
            }
        }
    }
}