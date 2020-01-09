using OpenNos.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.GameObject.Packets.ServerPackets
{
    [PacketHeader("title")]
    public class TitlePacket : PacketDefinition
    {
        [PacketIndex(0)]
        public List<TitleSubPacket> Data { get; set; }
    }
}
