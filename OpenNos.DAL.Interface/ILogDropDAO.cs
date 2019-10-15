using OpenNos.Data;
using OpenNos.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.Interface
{
    public interface ILogDropDAO
    {
        SaveResult InsertOrUpdate(ref LogDropDTO questLog);

        LogDropDTO LoadById(long id);

        IEnumerable<LogDropDTO> LoadByCharacterId(long id);
    }
}
