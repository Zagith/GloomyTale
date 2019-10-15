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
        LogDropDTO LoadById(long id);

        LogDropDTO Insert(LogDropDTO generalLog);

        IEnumerable<LogDropDTO> LoadByCharacterId(long id);
    }
}
