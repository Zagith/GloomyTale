using OpenNos.Data.Interfaces;
using OpenNos.Domain.I18N;

namespace OpenNos.Data.I18N
{
    public class I18NShopNameDto : II18NDto
    {
        public int I18NShopNameId { get; set; }

        public string Key { get; set; }
        public RegionType RegionType { get; set; }
        public string Text { get; set; }
    }
}
