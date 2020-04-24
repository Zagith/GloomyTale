using OpenNos.DAL.EF.Entities;
using OpenNos.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Mapper.Mappers
{
    public class RaidLogMapper
    {
        #region Methods

        public static bool ToRaidLog(RaidLogDTO input, RaidLog output)
        {
            if (input == null)
            {
                return false;
            }

            output.LogId = input.LogId;
            output.CharacterId = input.CharacterId;
            output.RaidId = input.RaidId;
            output.Timestamp = input.Timestamp;

            return true;
        }

        public static bool ToRaidLogDTO(RaidLog input, RaidLogDTO output)
        {
            if (input == null)
            {
                return false;
            }

            output.LogId = input.LogId;
            output.CharacterId = input.CharacterId;
            output.RaidId = input.RaidId;
            output.Timestamp = input.Timestamp;

            return true;
        }

        #endregion
    }
}
