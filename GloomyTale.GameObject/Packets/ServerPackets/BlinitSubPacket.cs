using GloomyTale.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.GameObject.Packets.ServerPackets
{
    public class BlinitSubPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public long RelatedCharacterId { get; set; }
        [PacketIndex(1)]
        public string CharacterName { get; set; }
    }
}
