using OpenNos.DAL.EF;
using OpenNos.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Mapper.Mappers
{
    public static class PvPLogMapper
    {
        #region Methods

        public static bool ToPvPLog(PvPLogDTO input, PvPLog output)
        {
            if (input == null)
            {
                return false;
            }

            output.LogId = input.LogId;
            output.CharacterId = input.CharacterId;
            output.TargetId = input.TargetId;
            output.IpAddress = input.IpAddress;
            output.Timestamp = input.Timestamp;

            return true;
        }

        public static bool ToPvPLogDTO(PvPLog input, PvPLogDTO output)
        {
            if (input == null)
            {
                return false;
            }

            output.LogId = input.LogId;
            output.CharacterId = input.CharacterId;
            output.TargetId = input.TargetId;
            output.IpAddress = input.IpAddress;
            output.Timestamp = input.Timestamp;

            return true;
        }

        #endregion
    }
}
