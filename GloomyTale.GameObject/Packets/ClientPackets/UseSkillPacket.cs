﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using GloomyTale.Core;
using GloomyTale.Domain;

namespace GloomyTale.GameObject
{
    [PacketHeader("u_s")]
    public class UseSkillPacket : PacketDefinition
    {
        #region Properties        

        [PacketIndex(0)]
        public int CastId { get; set; }

        [PacketIndex(1)]
        public VisualType UserType { get; set; }

        [PacketIndex(2)]
        public int MapMonsterId { get; set; }

        [PacketIndex(3)]
        public short? MapX { get; set; }

        [PacketIndex(4)]
        public short? MapY { get; set; }

        public override string ToString()
        {
            return $"{CastId} {UserType} {MapMonsterId} {MapX} {MapY}";
        }

        #endregion
    }
}