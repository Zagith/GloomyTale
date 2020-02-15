using GloomyTale.Data;
using GloomyTale.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloomyTale.DAL.Interface
{
    public interface ILogDropDAO : IMappingBaseDAO
    {        
        LogDropDTO LoadById(long id);

        LogDropDTO Insert(LogDropDTO generalLog);

        IEnumerable<LogDropDTO> LoadByCharacterId(long id);
    }
}
