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
    public class LogCommandsDAO : MappingBaseDao<LogCommands, LogCommandsDTO>, ILogCommandsDAO
    {
        public LogCommandsDAO(IMapper mapper) : base(mapper)
        { }

        public LogCommandsDTO Insert(LogCommandsDTO generalLog)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<LogCommands>(generalLog);
                    context.LogCommands.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<LogCommandsDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public LogCommandsDTO LoadById(long id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    LogCommands log = context.LogCommands.FirstOrDefault(a => a.CommandId.Equals(id));
                    if (log != null)
                    {
                        return _mapper.Map<LogCommandsDTO>(log);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
            return null;
        }

        public IEnumerable<LogCommandsDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (LogCommands id in context.LogCommands.Where(c => c.CharacterId == characterId))
                {
                    yield return _mapper.Map<LogCommandsDTO>(id);
                }
            }
        }
    }
}
