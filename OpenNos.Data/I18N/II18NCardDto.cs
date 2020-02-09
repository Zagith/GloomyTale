using OpenNos.Data.Interfaces;
using OpenNos.Domain.I18N;

namespace OpenNos.Data.I18N
{
    public class II18NCardDto : II18NDto
    {
        public int I18NCardId { get; set; }

        public string Key { get; set; }
        public RegionType RegionType { get; set; }
        public string Text { get; set; }
    }
}
