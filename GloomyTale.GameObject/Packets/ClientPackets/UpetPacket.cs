﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using GloomyTale.Core;
using GloomyTale.Domain;

namespace GloomyTale.GameObject
{
    [PacketHeader("u_pet")]
    public class UpetPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public int MateTransportId { get; set; }

        [PacketIndex(1)]
        public UserType TargetType { get; set; }

        [PacketIndex(2)]
        public int TargetId { get; set; }

        [PacketIndex(3)]
        public int Unknown2 { get; set; }

        public override string ToString() => $"{MateTransportId} {TargetType} {TargetId} 0";
    }
}