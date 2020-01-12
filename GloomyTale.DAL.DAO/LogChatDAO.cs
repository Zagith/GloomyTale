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
using System.Text;
using System.Threading.Tasks;

namespace GloomyTale.DAL.DAO
{
    public class LogChatDAO : ILogChatDAO
    {
        public LogChatDAO() : base()
        { }

        public LogChatDTO Insert(LogChatDTO generalLog)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    LogChat entity = new LogChat();
                    Mapper.Mappers.LogChatMapper.ToLogChat(generalLog, entity);
                    context.LogChat.Add(entity);
                    context.SaveChanges();
                    if (Mapper.Mappers.LogChatMapper.ToLogChatDTO(entity, generalLog))
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

        public LogChatDTO Update(LogChat old, LogChatDTO replace, OpenNosContext context)
        {
            if (old != null)
            {
                Mapper.Mappers.LogChatMapper.ToLogChat(replace, old);
                context.Entry(old).State = EntityState.Modified;
                context.SaveChanges();
            }
            if (Mapper.Mappers.LogChatMapper.ToLogChatDTO(old, replace))
            {
                return replace;
            }

            return null;
        }

        public LogChatDTO LoadById(long id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    LogChatDTO dto = new LogChatDTO();
                    if (Mapper.Mappers.LogChatMapper.ToLogChatDTO(context.LogChat.FirstOrDefault(i => i.LogId.Equals(id)), dto))
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

        public IEnumerable<LogChatDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<LogChatDTO> result = new List<LogChatDTO>();
                foreach (LogChat questLog in context.LogChat.Where(s => s.CharacterId == characterId))
                {
                    LogChatDTO dto = new LogChatDTO();
                    Mapper.Mappers.LogChatMapper.ToLogChatDTO(questLog, dto);
                    result.Add(dto);
                }
                return result;
            }
        }
    }
}
