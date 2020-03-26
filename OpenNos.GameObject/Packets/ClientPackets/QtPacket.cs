using OpenNos.Core;

namespace OpenNos.GameObject.Packets.ClientPackets
{
    [PacketHeader("qt")]
    public class QtPacket
    {
        #region Properties

        public short Type { get; set; }

        public int Data { get; set; }

        #endregion
    }
}
