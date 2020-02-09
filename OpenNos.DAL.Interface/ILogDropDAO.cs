using OpenNos.Data;
using System.Collections.Generic;

namespace OpenNos.DAL.Interface
{
    public interface ILogDropDAO
    {
        LogDropDTO LoadById(long id);

        LogDropDTO Insert(LogDropDTO generalLog);

        IEnumerable<LogDropDTO> LoadByCharacterId(long id);
    }
}
