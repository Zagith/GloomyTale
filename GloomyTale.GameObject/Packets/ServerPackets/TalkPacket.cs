﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)
using GloomyTale.Core;

namespace GloomyTale.GameObject.Packets.ServerPackets
{
    [PacketHeader("talk")]
    public class TalkPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public long CharacterId { get; set; }

        [PacketIndex(1)]
        public string Message { get; set; }

        #endregion
    }
}