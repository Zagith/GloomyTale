using OpenNos.Core;
using System.Collections.Generic;

namespace OpenNos.GameObject.Packets.ServerPackets
{
    [PacketHeader("title")]
    public class TitlePacket
    {
        [PacketIndex(0)]
        public List<TitleSubPacket> Data { get; set; }
    }
}
