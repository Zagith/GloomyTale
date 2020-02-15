using OpenNos.Data;
using System.Collections.Generic;

namespace OpenNos.DAL.Interface
{
    public interface ILogCommandsDAO
    {
        LogCommandsDTO Insert(LogCommandsDTO generalLog);

        LogCommandsDTO LoadById(long id);

        IEnumerable<LogCommandsDTO> LoadByCharacterId(long id);
    }
}
