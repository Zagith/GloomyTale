﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)
using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Domain;

namespace OpenNos.GameObject.Packets.ClientPackets
{
    [PacketHeader("up_gr")]
    public class UpgradePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte UpgradeType { get; set; }

        [PacketIndex(1)]
        public InventoryType InventoryType { get; set; }

        [PacketIndex(2)]
        public byte Slot { get; set; }

        [PacketIndex(3)]
        public InventoryType? InventoryType2 { get; set; }

        [PacketIndex(4)]
        public byte? Slot2 { get; set; }

        #endregion
    }
}
