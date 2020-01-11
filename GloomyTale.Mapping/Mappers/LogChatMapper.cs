using GloomyTale.DAL.EF;
using OpenNos.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
