using OpenNos.Data.Interfaces;
using OpenNos.Domain.I18N;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Data.I18N
{
    public class I18NTorFDto : II18NDto
    {
        public int I18NTorFId { get; set; }

        public string Key { get; set; }
        public RegionType RegionType { get; set; }
        public string Text { get; set; }
    }
}
