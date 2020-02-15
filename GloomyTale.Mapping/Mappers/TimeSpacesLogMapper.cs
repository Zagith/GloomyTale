using GloomyTale.DAL.EF.Entities;
using GloomyTale.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloomyTale.Mapper.Mappers
{
    public class TimeSpacesLogMapper
    {
        #region Methods

        public static bool ToTimeSpacesLog(TimeSpacesLogDTO input, TimeSpacesLog output)
        {
            if (input == null)
            {
                return false;
            }

            output.LogId = input.LogId;
            output.CharacterId = input.CharacterId;
            output.TimeSpaceId = input.TimeSpaceId;
            output.Timestamp = input.Timestamp;

            return true;
        }

        public static bool ToTimeSpacesLogDTO(TimeSpacesLog input, TimeSpacesLogDTO output)
        {
            if (input == null)
            {
                return false;
            }

            output.LogId = input.LogId;
            output.CharacterId = input.CharacterId;
            output.TimeSpaceId = input.TimeSpaceId;
            output.Timestamp = input.Timestamp;

            return true;
        }

        #endregion
    }
}
