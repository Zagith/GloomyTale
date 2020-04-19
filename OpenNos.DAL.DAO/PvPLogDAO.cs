using OpenNos.Core;
using OpenNos.DAL.EF;
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
    public class PvPLogDAO : IPvPLogDAO
    {
        public PvPLogDTO Insert(PvPLogDTO generalLog)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    PvPLog entity = new PvPLog();
                    Mapper.Mappers.PvPLogMapper.ToPvPLog(generalLog, entity);
                    context.PvPLog.Add(entity);
                    context.SaveChanges();
                    if (Mapper.Mappers.PvPLogMapper.ToPvPLogDTO(entity, generalLog))
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

        public PvPLogDTO Update(PvPLog old, PvPLogDTO replace, OpenNosContext context)
        {
            if (old != null)
            {
                Mapper.Mappers.PvPLogMapper.ToPvPLog(replace, old);
                context.Entry(old).State = EntityState.Modified;
                context.SaveChanges();
            }
            if (Mapper.Mappers.PvPLogMapper.ToPvPLogDTO(old, replace))
            {
                return replace;
            }

            return null;
        }

        public PvPLogDTO LoadById(long id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    PvPLogDTO dto = new PvPLogDTO();
                    if (Mapper.Mappers.PvPLogMapper.ToPvPLogDTO(context.PvPLog.FirstOrDefault(i => i.LogId.Equals(id)), dto))
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

        public IEnumerable<PvPLogDTO> LoadByCharacterId(long characterId, long targetId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<PvPLogDTO> result = new List<PvPLogDTO>();
                foreach (PvPLog questLog in context.PvPLog.Where(s => s.CharacterId == characterId && s.TargetId == targetId))
                {
                    PvPLogDTO dto = new PvPLogDTO();
                    Mapper.Mappers.PvPLogMapper.ToPvPLogDTO(questLog, dto);
                    result.Add(dto);
                }
                return result;
            }
        }
    }
}
