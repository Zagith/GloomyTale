using OpenNos.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.Interface
{
    public interface IRaidLogDAO
    {
        RaidLogDTO Insert(RaidLogDTO generalLog);

        IEnumerable<RaidLogDTO> LoadByCharacterId(long id);
    }
}
