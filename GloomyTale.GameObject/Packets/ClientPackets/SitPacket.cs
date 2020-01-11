﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using GloomyTale.Core;
using System.Collections.Generic;

namespace GloomyTale.GameObject
{
    [PacketHeader("rest")]
    public class SitPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public short Amount { get; set; }

        [PacketIndex(1, RemoveSeparator = true)]
        public List<SitSubPacket> Users { get; set; }

        #endregion
    }

    [PacketHeader("sit_sub_packet")] // header will be ignored for serializing just sub list packets
    public class SitSubPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public byte UserType { get; set; }

        [PacketIndex(1)]
        public long UserId { get; set; }
    }
}