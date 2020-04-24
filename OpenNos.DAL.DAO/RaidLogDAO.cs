using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Entities;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.DAO
{
    public class RaidLogDAO : IRaidLogDAO
    {
        public RaidLogDTO Insert(RaidLogDTO generalLog)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    RaidLog entity = new RaidLog();
                    Mapper.Mappers.RaidLogMapper.ToRaidLog(generalLog, entity);
                    context.RaidLog.Add(entity);
                    context.SaveChanges();
                    if (Mapper.Mappers.RaidLogMapper.ToRaidLogDTO(entity, generalLog))
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

        public RaidLogDTO Update(RaidLog old, RaidLogDTO replace, OpenNosContext context)
        {
            if (old != null)
            {
                Mapper.Mappers.RaidLogMapper.ToRaidLog(replace, old);
                context.Entry(old).State = EntityState.Modified;
                context.SaveChanges();
            }
            if (Mapper.Mappers.RaidLogMapper.ToRaidLogDTO(old, replace))
            {
                return replace;
            }

            return null;
        }

        public RaidLogDTO LoadById(long id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    RaidLogDTO dto = new RaidLogDTO();
                    if (Mapper.Mappers.RaidLogMapper.ToRaidLogDTO(context.RaidLog.FirstOrDefault(i => i.LogId.Equals(id)), dto))
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

        public IEnumerable<RaidLogDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<RaidLogDTO> result = new List<RaidLogDTO>();
                foreach (RaidLog questLog in context.RaidLog.Where(s => s.CharacterId == characterId))
                {
                    RaidLogDTO dto = new RaidLogDTO();
                    Mapper.Mappers.RaidLogMapper.ToRaidLogDTO(questLog, dto);
                    result.Add(dto);
                }
                return result;
            }
        }
    }
}
