﻿using GloomyTale.Core;
using GloomyTale.Domain;

namespace GloomyTale.GameObject.Packets.ClientPackets
{
    [PacketHeader("u_ps")]
    public class UsePartnerSkillPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public long TransportId { get; set; }

        [PacketIndex(1)]
        public VisualType TargetType { get; set; }

        [PacketIndex(2)]
        public long TargetId { get; set; }

        [PacketIndex(3)]
        public byte CastId { get; set; }
        
        [PacketIndex(4)]
        public short MapX { get; set; }

        [PacketIndex(5)]
        public short MapY { get; set; }

        #endregion
    }
}
