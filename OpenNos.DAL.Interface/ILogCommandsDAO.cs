using OpenNos.Data;
using OpenNos.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.Interface
{
    public interface ILogCommandsDAO
    {
        SaveResult InsertOrUpdate(ref LogCommandsDTO questLog);

        LogCommandsDTO LoadById(long id);

        IEnumerable<LogCommandsDTO> LoadByCharacterId(long id);
    }
}
