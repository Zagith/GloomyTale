using OpenNos.Core;
using System.Collections.Generic;
using OpenNos.Core.Serializing;

namespace OpenNos.GameObject.Packets.ServerPackets
{
    [PacketHeader("title")]
    public class TitlePacket : PacketDefinition
    {
        [PacketIndex(0)]
        public List<TitleSubPacket> Data { get; set; }
    }
}
