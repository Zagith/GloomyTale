﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using System.Collections.Generic;
using GloomyTale.Core;
using GloomyTale.Domain;

namespace GloomyTale.GameObject
{
    [PacketHeader("mtlist")]
    public class MultiTargetListPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte TargetsAmount { get; set; }

        [PacketIndex(1, RemoveSeparator = true)]
        public List<MultiTargetListSubPacket> Targets { get; set; }

        #endregion
    }

    [PacketHeader("mtlist_sub_packet")] // header will be ignored for serializing just sub list packets
    public class MultiTargetListSubPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public UserType TargetType { get; set; }

        [PacketIndex(1)]
        public int TargetId { get; set; }
    }
}