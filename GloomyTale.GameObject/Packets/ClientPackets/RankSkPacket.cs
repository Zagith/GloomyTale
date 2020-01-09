using OpenNos.Core;

namespace OpenNos.GameObject.Packets.ClientPackets
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
