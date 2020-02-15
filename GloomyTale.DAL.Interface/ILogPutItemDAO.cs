using GloomyTale.Data;
using GloomyTale.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloomyTale.DAL.Interface
{
    public interface ILogPutItemDAO : IMappingBaseDAO
    {
        LogPutItemDTO Insert(LogPutItemDTO generalLog);

        LogPutItemDTO LoadById(long id);

        IEnumerable<LogPutItemDTO> LoadByCharacterId(long id);
    }
}
