using GloomyTale.Data;
using GloomyTale.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloomyTale.DAL.Interface
{
    public interface ILogCommandsDAO : IMappingBaseDAO
    {
        LogCommandsDTO Insert(LogCommandsDTO generalLog);

        LogCommandsDTO LoadById(long id);

        IEnumerable<LogCommandsDTO> LoadByCharacterId(long id);
    }
}
