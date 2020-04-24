using OpenNos.DAL.EF.Entities;
using OpenNos.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Mapper.Mappers
{
    public class UpgradeLogMapper
    {
        #region Methods

        public static bool ToUpgradeLog(UpgradeLogDTO input, UpgradeLog output)
        {
            if (input == null)
            {
                return false;
            }

            output.LogId = input.LogId;
            output.CharacterId = input.CharacterId;
            output.EquipmentSerialized = input.EquipmentSerialized;
            output.Upgrade = input.Upgrade;
            output.Timestamp = input.Timestamp;

            return true;
        }

        public static bool ToUpgradeLogDTO(UpgradeLog input, UpgradeLogDTO output)
        {
            if (input == null)
            {
                return false;
            }

            output.LogId = input.LogId;
            output.CharacterId = input.CharacterId;
            output.EquipmentSerialized = input.EquipmentSerialized;
            output.Upgrade = input.Upgrade;
            output.Timestamp = input.Timestamp;

            return true;
        }

        #endregion
    }
}
