using GloomyTale.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.GameObject.Packets.ServerPackets
{
    [PacketHeader("blinit")]
    public class BlinitPacket : PacketDefinition
    {
        [PacketIndex(0, SpecialSeparator = "|")]
        public List<BlinitSubPacket> SubPackets { get; set; }
    }
}
