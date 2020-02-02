﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;

namespace OpenNos.GameObject
{
    [PacketHeader("pt_ctl_sub_packet")] // header will be ignored for serializing just sub list packets
    public class PtCtlSubPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public long UserId { get; set; }

        [PacketIndex(1)]
        public short UserX { get; set; }

        [PacketIndex(2)]
        public short UserY { get; set; }
    }
}