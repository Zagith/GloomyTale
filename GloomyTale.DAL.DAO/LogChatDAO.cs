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
using AutoMapper;

namespace GloomyTale.DAL.DAO
{
    public class LogChatDAO : MappingBaseDao<LogChat, LogChatDTO>, ILogChatDAO
    {
        public LogChatDAO(IMapper mapper) : base(mapper)
        { }

        public LogChatDTO Insert(LogChatDTO generalLog)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<LogChat>(generalLog);
                    context.LogChat.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<LogChatDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public LogChatDTO LoadById(long id)
        {
            try
            { 
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    LogChat log = context.LogChat.FirstOrDefault(a => a.LogId.Equals(id));
                    if (log != null)
                    {
                        return _mapper.Map<LogChatDTO>(log);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
            return null;
        }

        public IEnumerable<LogChatDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (LogChat id in context.LogChat.Where(c => c.CharacterId == characterId))
                {
                    yield return _mapper.Map<LogChatDTO>(id);
                }
            }
        }
    }
}
