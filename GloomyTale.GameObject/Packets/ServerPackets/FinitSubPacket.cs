using GloomyTale.Core;
using GloomyTale.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.GameObject.Packets.ServerPackets
{
    [PacketHeader("finit_sub_packet")]
    public class FinitSubPacket : PacketDefinition
    {

        [PacketIndex(0)]
        public long CharacterId { get; set; }
        [PacketIndex(1)]
        public CharacterRelationType RelationType { get; set; }
        [PacketIndex(2)]
        public bool IsOnline { get; set; }
        [PacketIndex(3)]
        public string CharacterName { get; set; }
    }
}
