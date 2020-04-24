using OpenNos.DAL.EF.Entities;
using OpenNos.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Mapper.Mappers
{
    public class LevelLogMapper
    {
        #region Methods

        public static bool ToLevelLog(LevelLogDTO input, LevelLog output)
        {
            if (input == null)
            {
                return false;
            }

            output.LogId = input.LogId;
            output.CharacterId = input.CharacterId;
            output.Level = input.Level;
            output.Timestamp = input.Timestamp;

            return true;
        }

        public static bool ToLevelLogDTO(LevelLog input, LevelLogDTO output)
        {
            if (input == null)
            {
                return false;
            }

            output.LogId = input.LogId;
            output.CharacterId = input.CharacterId;
            output.Level = input.Level;
            output.Timestamp = input.Timestamp;

            return true;
        }

        #endregion
    }
}
