﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using GloomyTale.Core;

namespace GloomyTale.GameObject
{
    [PacketHeader("mjoin")]
    public class MJoinPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte Type { get; set; }
        [PacketIndex(1)]
        public long CharacterId { get; set; }
        #endregion
    }
}