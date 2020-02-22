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
    public class LogPutItemDAO : MappingBaseDao<LogPutItem, LogPutItemDTO>, ILogPutItemDAO
    {
        public LogPutItemDAO(IMapper mapper) : base(mapper)
        { }

        public LogPutItemDTO Insert(LogPutItemDTO generalLog)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<LogPutItem>(generalLog);
                    context.LogPutItem.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<LogPutItemDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public LogPutItemDTO LoadById(long id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    LogPutItem log = context.LogPutItem.FirstOrDefault(a => a.LogId.Equals(id));
                    if (log != null)
                    {
                        return _mapper.Map<LogPutItemDTO>(log);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
            return null;
        }

        public IEnumerable<LogPutItemDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (LogPutItem id in context.LogPutItem.Where(c => c.CharacterId == characterId))
                {
                    yield return _mapper.Map<LogPutItemDTO>(id);
                }
            }
        }
    }
}
