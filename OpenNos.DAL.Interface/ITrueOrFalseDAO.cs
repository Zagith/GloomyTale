using OpenNos.Data;
using System.Collections.Generic;

namespace OpenNos.DAL.Interface
{
    public interface ITrueOrFalseDAO
    {
        IEnumerable<TrueOrFalseDTO> LoadByType(short questionType);
    }
}
