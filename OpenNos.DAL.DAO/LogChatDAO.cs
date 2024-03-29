﻿using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace OpenNos.DAL.DAO
{
    public class LogChatDAO : ILogChatDAO
    {
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
                Logger.Error(e);
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
                Logger.Error(e);
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
