﻿//// <auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;

namespace OpenNos.GameObject
{
    [PacketHeader("ta_call")]
    public class TaCallPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte CalledIndex { get; set; }

        #endregion
    }
}