using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Entities;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace OpenNos.DAL.DAO
{
    public class LevelLogDAO : ILevelLogDAO
    {
        public LevelLogDTO Insert(LevelLogDTO generalLog)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    LevelLog entity = new LevelLog();
                    Mapper.Mappers.LevelLogMapper.ToLevelLog(generalLog, entity);
                    context.LevelLog.Add(entity);
                    context.SaveChanges();
                    if (Mapper.Mappers.LevelLogMapper.ToLevelLogDTO(entity, generalLog))
                    {
                        return generalLog;
                    }

                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public LevelLogDTO Update(LevelLog old, LevelLogDTO replace, OpenNosContext context)
        {
            if (old != null)
            {
                Mapper.Mappers.LevelLogMapper.ToLevelLog(replace, old);
                context.Entry(old).State = EntityState.Modified;
                context.SaveChanges();
            }
            if (Mapper.Mappers.LevelLogMapper.ToLevelLogDTO(old, replace))
            {
                return replace;
            }

            return null;
        }

        public LevelLogDTO LoadById(long id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    LevelLogDTO dto = new LevelLogDTO();
                    if (Mapper.Mappers.LevelLogMapper.ToLevelLogDTO(context.LevelLog.FirstOrDefault(i => i.LogId.Equals(id)), dto))
                    {
                        return dto;
                    }

                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<LevelLogDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<LevelLogDTO> result = new List<LevelLogDTO>();
                foreach (LevelLog questLog in context.LevelLog.Where(s => s.CharacterId == characterId))
                {
                    LevelLogDTO dto = new LevelLogDTO();
                    Mapper.Mappers.LevelLogMapper.ToLevelLogDTO(questLog, dto);
                    result.Add(dto);
                }
                return result;
            }
        }
    }
}
