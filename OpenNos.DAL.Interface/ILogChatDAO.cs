using OpenNos.Data;
using OpenNos.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.Interface
{
    public interface ILogChatDAO
    {
        LogChatDTO Insert(LogChatDTO generalLog);

        LogChatDTO LoadById(long id);

        IEnumerable<LogChatDTO> LoadByCharacterId(long id);
    }
}
