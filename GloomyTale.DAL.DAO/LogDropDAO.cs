using GloomyTale.Core;
using GloomyTale.DAL.EF;
using GloomyTale.DAL.EF.Helpers;
using GloomyTale.DAL.Interface;
using GloomyTale.Data;
using GloomyTale.Data.Enums;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GloomyTale.DAL.DAO
{
    public class LogDropDAO : ILogDropDAO
    {
        public LogDropDAO() : base()
        { }

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
                Logger.Log.Error(e);
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
                Logger.Log.Error(e);
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
