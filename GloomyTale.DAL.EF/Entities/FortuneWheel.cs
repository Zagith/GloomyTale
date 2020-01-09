using System.ComponentModel.DataAnnotations;

namespace OpenNos.DAL.EF.Entities
{
    public class FortuneWheel
    {
        [Key]
        public short TentaLaFortunaId { get; set; }

        public bool IsRareRandom { get; set; }

        public byte Rare { get; set; }

        public byte Upgrade { get; set; }

        public ushort ItemGeneratedAmount { get; set; }

        public virtual Item Item { get; set; }

        public short ItemGeneratedVNum { get; set; }

        public short Probability { get; set; }

        public virtual Shop Shop { get; set; }

        public short ShopId { get; set; }
    }
}
