﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;

namespace OpenNos.GameObject
{
    [PacketHeader("mkraid")]
    public class MkRaidPacket : PacketDefinition
    {
        #region Properties        

        [PacketIndex(0)]
        public byte Type { get; set; }

        [PacketIndex(1)]
        public short Parameter { get; set; }

        #endregion
    }
}