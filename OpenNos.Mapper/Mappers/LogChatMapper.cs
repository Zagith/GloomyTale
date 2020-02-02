using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Entities;
using OpenNos.Data;

namespace OpenNos.Mapper.Mappers
{
    public class LogChatMapper
    {
        #region Methods

        public static bool ToLogChat(LogChatDTO input, LogChat output)
        {
            if (input == null)
            {
                return false;
            }

            output.LogId = input.LogId;
            output.CharacterId = input.CharacterId;
            output.ChatType = input.ChatType;
            output.ChatMessage = input.ChatMessage;
            output.IpAddress = input.IpAddress;
            output.Timestamp = input.Timestamp;

            return true;
        }

        public static bool ToLogChatDTO(LogChat input, LogChatDTO output)
        {
            if (input == null)
            {
                return false;
            }

            output.LogId = input.LogId;
            output.CharacterId = input.CharacterId;
            output.ChatType = input.ChatType;
            output.ChatMessage = input.ChatMessage;
            output.IpAddress = input.IpAddress;
            output.Timestamp = input.Timestamp;

            return true;
        }

        #endregion
    }
}
