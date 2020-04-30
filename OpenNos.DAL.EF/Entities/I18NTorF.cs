using OpenNos.Domain.I18N;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.DAL.EF.Entities
{
    public class I18NTorF
    {
        [Key]
        public int I18NTorFId { get; set; }

        public string Key { get; set; }
        public RegionType RegionType { get; set; }
        public string Text { get; set; }
    }
}
