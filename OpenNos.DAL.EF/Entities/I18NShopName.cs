using OpenNos.Domain.I18N;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.DAL.EF.Entities
{
    public class I18NShopName
    {
        [Key]
        public int I18NShopNameId { get; set; }

        public string Key { get; set; }
        public RegionType RegionType { get; set; }
        public string Text { get; set; }
    }
}
