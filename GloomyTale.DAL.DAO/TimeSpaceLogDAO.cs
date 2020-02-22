using GloomyTale.Core;
using GloomyTale.DAL.EF;
using GloomyTale.DAL.EF.Entities;
using GloomyTale.DAL.EF.Helpers;
using GloomyTale.DAL.Interface;
using GloomyTale.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace GloomyTale.DAL.DAO
{
    public class TimeSpaceLogDAO : MappingBaseDao<TimeSpacesLog, TimeSpacesLogDTO>, ITimeSpaceLogDAO
    {
        public TimeSpaceLogDAO(IMapper mapper) : base(mapper)
        { }

        public TimeSpacesLogDTO Insert(TimeSpacesLogDTO generalLog)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<TimeSpacesLog>(generalLog);
                    context.TimeSpacesLog.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<TimeSpacesLogDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<TimeSpacesLogDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (TimeSpacesLog home in context.TimeSpacesLog.Where(s => s.CharacterId == characterId))
                {
                    yield return _mapper.Map<TimeSpacesLogDTO>(home);
                }
            }
        }
    }
}
