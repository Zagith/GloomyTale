using GloomyTale.Core;
using GloomyTale.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.GameObject.Packets.ServerPackets
{
    [PacketHeader("npc_req")]
    public class RequestNpcPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public VisualType Type { get; set; }
        [PacketIndex(1)]
        public long TargetId { get; set; }
        [PacketIndex(2)]
        public long? Data { get; set; }
    }
}