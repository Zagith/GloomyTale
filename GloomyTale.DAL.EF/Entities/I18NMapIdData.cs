using OpenNos.Domain.I18N;
using System.ComponentModel.DataAnnotations;

namespace GloomyTale.DAL.EF.Entities
{
    public class I18NMapIdData
    {
        [Key]
        public int I18NMapIdDataId { get; set; }

        public string Key { get; set; }
        public RegionType RegionType { get; set; }
        public string Text { get; set; }
    }
}
