using OpenNos.Domain.I18N;
using System.Collections.Generic;

namespace OpenNos.Data.Base
{
    public class I18NString : Dictionary<RegionType, string>
    {
        public I18NString()
        {
            Add(RegionType.EN, "NONAME");
            Add(RegionType.DE, "NONAME");
            Add(RegionType.FR, "NONAME");
            Add(RegionType.IT, "NONAME");
            Add(RegionType.PL, "NONAME");
            Add(RegionType.ES, "NONAME");
            Add(RegionType.CS, "NONAME");
            Add(RegionType.TR, "NONAME");
            Add(RegionType.RU, "NONAME");
        }
    }
}
