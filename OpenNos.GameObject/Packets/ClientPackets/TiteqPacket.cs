using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.Packets.ClientPackets
{
    [PacketHeader("tit_eq")]
    public class TiteqPacket
    {
        #region Properties

        [PacketIndex(0)]
        public TiteqPacketType Type { get; set; }

        [PacketIndex(1)]
        public short TitleVNum { get; set; }

        #endregion
    }
}
