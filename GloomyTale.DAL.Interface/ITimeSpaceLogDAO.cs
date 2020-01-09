using OpenNos.Data;
using System.Collections.Generic;

namespace OpenNos.DAL.Interface
{
    public interface ITimeSpaceLogDAO
    {
        TimeSpacesLogDTO Insert(TimeSpacesLogDTO generalLog);

        IEnumerable<TimeSpacesLogDTO> LoadByCharacterId(long id);
    }
}
