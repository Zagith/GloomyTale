using OpenNos.DAL.EF;
using OpenNos.Data;

namespace OpenNos.Mapper.Mappers
{
    public static class LogCommandsMapper
    {
        #region Methods

        public static bool ToLogCommands(LogCommandsDTO input, LogCommands output)
        {
            if (input == null)
            {
                return false;
            }

            output.CommandId = input.CommandId;
            output.CharacterId = input.CharacterId;
            output.Command = input.Command;
            output.Data = input.Data;
            output.IpAddress = input.IpAddress;
            output.Timestamp = input.Timestamp;

            return true;
        }

        public static bool ToLogCommandsDTO(LogCommands input, LogCommandsDTO output)
        {
            if (input == null)
            {
                return false;
            }

            output.CommandId = input.CommandId;
            output.CharacterId = input.CharacterId;
            output.Command = input.Command;
            output.Data = input.Data;
            output.IpAddress = input.IpAddress;
            output.Timestamp = input.Timestamp;

            return true;
        }

        #endregion
    }
}
