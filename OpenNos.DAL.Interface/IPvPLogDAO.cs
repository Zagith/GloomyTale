using OpenNos.Data;
using System.Collections.Generic;

namespace OpenNos.DAL.Interface
{
    public interface IPvPLogDAO
    {
        PvPLogDTO Insert(PvPLogDTO generalLog);

        PvPLogDTO LoadById(long id);

        IEnumerable<PvPLogDTO> LoadByCharacterId(long id, long targetid);
    }
}
