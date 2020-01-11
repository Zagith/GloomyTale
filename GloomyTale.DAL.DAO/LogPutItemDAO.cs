using OpenNos.Core;
using GloomyTale.DAL.EF;
using GloomyTale.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using OpenNos.Data.Enums;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.DAO
{
    public class LogPutItemDAO : ILogPutItemDAO
    {
        public LogPutItemDTO Insert(LogPutItemDTO generalLog)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    LogPutItem entity = new LogPutItem();
                    Mapper.Mappers.LogPutItemMapper.ToLogPutItem(generalLog, entity);
                    context.LogPutItem.Add(entity);
                    context.SaveChanges();
                    if (Mapper.Mappers.LogPutItemMapper.ToLogPutItemDTO(entity, generalLog))
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

        public LogPutItemDTO Update(LogPutItem old, LogPutItemDTO replace, OpenNosContext context)
        {
            if (old != null)
            {
                Mapper.Mappers.LogPutItemMapper.ToLogPutItem(replace, old);
                context.Entry(old).State = EntityState.Modified;
                context.SaveChanges();
            }
            if (Mapper.Mappers.LogPutItemMapper.ToLogPutItemDTO(old, replace))
            {
                return replace;
            }

            return null;
        }

        public LogPutItemDTO LoadById(long id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    LogPutItemDTO dto = new LogPutItemDTO();
                    if (Mapper.Mappers.LogPutItemMapper.ToLogPutItemDTO(context.LogPutItem.FirstOrDefault(i => i.LogId.Equals(id)), dto))
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

        public IEnumerable<LogPutItemDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<LogPutItemDTO> result = new List<LogPutItemDTO>();
                foreach (LogPutItem questLog in context.LogPutItem.Where(s => s.CharacterId == characterId))
                {
                    LogPutItemDTO dto = new LogPutItemDTO();
                    Mapper.Mappers.LogPutItemMapper.ToLogPutItemDTO(questLog, dto);
                    result.Add(dto);
                }
                return result;
            }
        }
    }
}
