using GloomyTale.Data;
using GloomyTale.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloomyTale.DAL.Interface
{
    public interface ILogChatDAO : IMappingBaseDAO
    {
        LogChatDTO Insert(LogChatDTO generalLog);

        LogChatDTO LoadById(long id);

        IEnumerable<LogChatDTO> LoadByCharacterId(long id);
    }
}
