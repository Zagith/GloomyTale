﻿using GloomyTale.Core;

namespace GloomyTale.GameObject.Packets.ClientPackets
{
    [PacketHeader("fb")]
    public class FbPacket : PacketDefinition
    {
        #region Properties        

        [PacketIndex(0)]
        public short Type { get; set; }

        [PacketIndex(1)]
        public long CharacterId { get; set; }

        [PacketIndex(2)]
        public short? Parameter { get; set; }

        #endregion
    }
}
