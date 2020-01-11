using GloomyTale.Data;
using System.Collections.Generic;

namespace GloomyTale.DAL.Interface
{
    public interface ITimeSpaceLogDAO
    {
        TimeSpacesLogDTO Insert(TimeSpacesLogDTO generalLog);

        IEnumerable<TimeSpacesLogDTO> LoadByCharacterId(long id);
    }
}
