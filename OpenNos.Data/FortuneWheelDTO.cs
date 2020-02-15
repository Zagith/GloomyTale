namespace OpenNos.Data
{
    public class FortuneWheelDTO
    {
        public bool IsRareRandom { get; set; }

        public byte Rare { get; set; }

        public byte Upgrade { get; set; }

        public ushort ItemGeneratedAmount { get; set; }

        public short ItemGeneratedVNum { get; set; }

        public short Probability { get; set; }

        public short TentaLaFortunaId { get; set; }

        public short ShopId { get; set; }
    }
}
