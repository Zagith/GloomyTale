using OpenNos.Data;
using System.Collections.Generic;

namespace OpenNos.DAL.Interface
{
    public interface ILogPutItemDAO
    {
        LogPutItemDTO Insert(LogPutItemDTO generalLog);

        LogPutItemDTO LoadById(long id);

        IEnumerable<LogPutItemDTO> LoadByCharacterId(long id);
    }
}
