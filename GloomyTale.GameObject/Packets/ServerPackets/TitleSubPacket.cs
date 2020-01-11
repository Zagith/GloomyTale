﻿using GloomyTale.Core;

namespace GloomyTale.GameObject.Packets.ServerPackets
{
    [PacketHeader(" ")]
    public class TitleSubPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public short TitleId { get; set; }

        [PacketIndex(1)]
        public byte TitleStatus { get; set; }
    }
}
