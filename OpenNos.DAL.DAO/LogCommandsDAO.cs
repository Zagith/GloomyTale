using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using OpenNos.Data.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.DAO
{
    public class LogCommandsDAO : ILogCommandsDAO
    {
        public LogCommandsDTO Insert(LogCommandsDTO generalLog)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    LogCommands entity = new LogCommands();
                    Mapper.Mappers.LogCommandsMapper.ToLogCommands(generalLog, entity);
                    context.LogCommands.Add(entity);
                    context.SaveChanges();
                    if (Mapper.Mappers.LogCommandsMapper.ToLogCommandsDTO(entity, generalLog))
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

        public LogCommandsDTO Update(LogCommands old, LogCommandsDTO replace, OpenNosContext context)
        {
            if (old != null)
            {
                Mapper.Mappers.LogCommandsMapper.ToLogCommands(replace, old);
                context.Entry(old).State = EntityState.Modified;
                context.SaveChanges();
            }
            if (Mapper.Mappers.LogCommandsMapper.ToLogCommandsDTO(old, replace))
            {
                return replace;
            }

            return null;
        }

        public LogCommandsDTO LoadById(long id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    LogCommandsDTO dto = new LogCommandsDTO();
                    if (Mapper.Mappers.LogCommandsMapper.ToLogCommandsDTO(context.LogCommands.FirstOrDefault(i => i.CommandId.Equals(id)), dto))
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

        public IEnumerable<LogCommandsDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<LogCommandsDTO> result = new List<LogCommandsDTO>();
                foreach (LogCommands questLog in context.LogCommands.Where(s => s.CharacterId == characterId))
                {
                    LogCommandsDTO dto = new LogCommandsDTO();
                    Mapper.Mappers.LogCommandsMapper.ToLogCommandsDTO(questLog, dto);
                    result.Add(dto);
                }
                return result;
            }
        }
    }
}
