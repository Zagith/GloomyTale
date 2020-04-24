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
    public class UpgradeLogDAO : IUpgradeLogDAO
    {
        public UpgradeLogDTO Insert(UpgradeLogDTO generalLog)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    UpgradeLog entity = new UpgradeLog();
                    Mapper.Mappers.UpgradeLogMapper.ToUpgradeLog(generalLog, entity);
                    context.UpgradeLog.Add(entity);
                    context.SaveChanges();
                    if (Mapper.Mappers.UpgradeLogMapper.ToUpgradeLogDTO(entity, generalLog))
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

        public UpgradeLogDTO Update(UpgradeLog old, UpgradeLogDTO replace, OpenNosContext context)
        {
            if (old != null)
            {
                Mapper.Mappers.UpgradeLogMapper.ToUpgradeLog(replace, old);
                context.Entry(old).State = EntityState.Modified;
                context.SaveChanges();
            }
            if (Mapper.Mappers.UpgradeLogMapper.ToUpgradeLogDTO(old, replace))
            {
                return replace;
            }

            return null;
        }

        public UpgradeLogDTO LoadById(long id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    UpgradeLogDTO dto = new UpgradeLogDTO();
                    if (Mapper.Mappers.UpgradeLogMapper.ToUpgradeLogDTO(context.UpgradeLog.FirstOrDefault(i => i.LogId.Equals(id)), dto))
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

        public IEnumerable<UpgradeLogDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<UpgradeLogDTO> result = new List<UpgradeLogDTO>();
                foreach (UpgradeLog questLog in context.UpgradeLog.Where(s => s.CharacterId == characterId))
                {
                    UpgradeLogDTO dto = new UpgradeLogDTO();
                    Mapper.Mappers.UpgradeLogMapper.ToUpgradeLogDTO(questLog, dto);
                    result.Add(dto);
                }
                return result;
            }
        }
    }
}
