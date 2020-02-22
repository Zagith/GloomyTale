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
    public class LogDropDAO : MappingBaseDao<LogDrop, LogDropDTO>, ILogDropDAO
    {
        public LogDropDAO(IMapper mapper) : base(mapper)
        { }

        public LogDropDTO Insert(LogDropDTO generalLog)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<LogDrop>(generalLog);
                    context.LogDrop.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<LogDropDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public LogDropDTO LoadById(long id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    LogDrop log = context.LogDrop.FirstOrDefault(a => a.LogId.Equals(id));
                    if (log != null)
                    {
                        return _mapper.Map<LogDropDTO>(log);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
            return null;
        }

        public IEnumerable<LogDropDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (LogDrop id in context.LogDrop.Where(c => c.CharacterId == characterId))
                {
                    yield return _mapper.Map<LogDropDTO>(id);
                }
            }
        }
    }
}
