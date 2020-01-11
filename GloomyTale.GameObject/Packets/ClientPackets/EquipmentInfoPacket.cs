﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)
using GloomyTale.Core;

namespace GloomyTale.GameObject.Packets.ClientPackets
{
    [PacketHeader("eqinfo")]
    public class EquipmentInfoPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte Type { get; set; }

        [PacketIndex(1)]
        public short Slot { get; set; }

        [PacketIndex(2)]
        public short MateSlot { get; set; }

        [PacketIndex(3)]
        public long? ShopOwnerId { get; set; }

        #endregion
    }
}
