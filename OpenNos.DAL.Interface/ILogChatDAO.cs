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
        SaveResult InsertOrUpdate(ref LogChatDTO questLog);

        LogChatDTO LoadById(long id);

        IEnumerable<LogChatDTO> LoadByCharacterId(long id);
    }
}
