﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using GloomyTale.Core;

namespace GloomyTale.GameObject
{
    [PacketHeader("get")]
    public class GetPacket : PacketDefinition
    {
        #region Properties        
        [PacketIndex(0)]
        public byte PickerType { get; set; }

        [PacketIndex(1)]
        public int PickerId { get; set; }

        [PacketIndex(2)]
        public long TransportId { get; set; }

        #endregion
    }
}