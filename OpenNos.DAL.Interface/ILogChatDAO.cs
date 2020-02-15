using OpenNos.Data;
using System.Collections.Generic;

namespace OpenNos.DAL.Interface
{
    public interface ILogChatDAO
    {
        LogChatDTO Insert(LogChatDTO generalLog);

        LogChatDTO LoadById(long id);

        IEnumerable<LogChatDTO> LoadByCharacterId(long id);
    }
}
