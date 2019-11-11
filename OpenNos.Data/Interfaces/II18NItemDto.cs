using OpenNos.Domain.I18N;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Data.Interfaces
{
    public interface II18NItemDto
    {
        string Key { get; set; }
        RegionType RegionType { get; set; }
        string Text { get; set; }
    }
}
