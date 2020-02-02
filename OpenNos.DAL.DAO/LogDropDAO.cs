using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using OpenNos.DAL.EF.Context;
using OpenNos.DAL.EF.Entities;

namespace OpenNos.DAL.DAO
{
    public class LogDropDAO : ILogDropDAO
    {
        public LogDropDTO Insert(LogDropDTO generalLog)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    LogDrop entity = new LogDrop();
                    Mapper.Mappers.LogDropMapper.ToLogDrop(generalLog, entity);
                    context.LogDrop.Add(entity);
                    context.SaveChanges();
                    if (Mapper.Mappers.LogDropMapper.ToLogDropDTO(entity, generalLog))
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

        public LogDropDTO Update(LogDrop old, LogDropDTO replace, OpenNosContext context)
        {
            if (old != null)
            {
                Mapper.Mappers.LogDropMapper.ToLogDrop(replace, old);
                context.Entry(old).State = EntityState.Modified;
                context.SaveChanges();
            }
            if (Mapper.Mappers.LogDropMapper.ToLogDropDTO(old, replace))
            {
                return replace;
            }

            return null;
        }

        public LogDropDTO LoadById(long id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    LogDropDTO dto = new LogDropDTO();
                    if (Mapper.Mappers.LogDropMapper.ToLogDropDTO(context.LogDrop.FirstOrDefault(i => i.LogId.Equals(id)), dto))
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

        public IEnumerable<LogDropDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<LogDropDTO> result = new List<LogDropDTO>();
                foreach (LogDrop questLog in context.LogDrop.Where(s => s.CharacterId == characterId))
                {
                    LogDropDTO dto = new LogDropDTO();
                    Mapper.Mappers.LogDropMapper.ToLogDropDTO(questLog, dto);
                    result.Add(dto);
                }
                return result;
            }
        }
    }
}
