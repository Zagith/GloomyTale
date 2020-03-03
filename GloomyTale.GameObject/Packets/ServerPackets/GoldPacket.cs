using GloomyTale.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.GameObject.Packets.ServerPackets
{
    [PacketHeader("gold")]
    public class GoldPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public long Gold { get; set; }
        [PacketIndex(1)]
        public int Unknown { get; set; }
    }
}
