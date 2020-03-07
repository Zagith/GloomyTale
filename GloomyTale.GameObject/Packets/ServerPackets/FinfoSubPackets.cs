using GloomyTale.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.GameObject.Packets.ServerPackets
{
    [PacketHeader("finfo_sub_packets")]
    public class FinfoSubPackets : PacketDefinition
    {

        [PacketIndex(0)]
        public long CharacterId { get; set; }
        [PacketIndex(1)]
        public bool IsConnected { get; set; }
    }
}
