using GloomyTale.Core;

namespace GloomyTale.GameObject.Packets.ClientPackets
{
    [PacketHeader("ranksk")]
    public class RankSkPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte Index { get; set; }

        #endregion
    }
}
