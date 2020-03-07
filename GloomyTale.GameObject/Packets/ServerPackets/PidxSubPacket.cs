using GloomyTale.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.GameObject.Packets.ServerPackets
{
    [PacketHeader("pidx_sub_packet")]
    public class PidxSubPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public bool IsGrouped { get; set; }
        [PacketIndex(1)]
        public long VisualId { get; set; }
    }
}