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

    [PacketHeader("blinit_sub_packet")]
    public class BlinitSubPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public long RelatedCharacterId { get; set; }
        [PacketIndex(1)]
        public string CharacterName { get; set; }
    }
}
