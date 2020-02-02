using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Entities;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenNos.DAL.EF.Context;

namespace OpenNos.DAL.DAO
{
    public class TimeSpaceLogDAO : ITimeSpaceLogDAO
    {
        public TimeSpacesLogDTO Insert(TimeSpacesLogDTO generalLog)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    TimeSpacesLog entity = new TimeSpacesLog();
                    Mapper.Mappers.TimeSpacesLogMapper.ToTimeSpacesLog(generalLog, entity);
                    context.TimeSpacesLog.Add(entity);
                    context.SaveChanges();
                    if (Mapper.Mappers.TimeSpacesLogMapper.ToTimeSpacesLogDTO(entity, generalLog))
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

        public IEnumerable<TimeSpacesLogDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<TimeSpacesLogDTO> result = new List<TimeSpacesLogDTO>();
                foreach (TimeSpacesLog questLog in context.TimeSpacesLog.Where(s => s.CharacterId == characterId))
                {
                    TimeSpacesLogDTO dto = new TimeSpacesLogDTO();
                    Mapper.Mappers.TimeSpacesLogMapper.ToTimeSpacesLogDTO(questLog, dto);
                    result.Add(dto);
                }
                return result;
            }
        }
    }
}
