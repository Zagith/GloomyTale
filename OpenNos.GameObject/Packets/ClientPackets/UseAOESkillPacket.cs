﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;

namespace OpenNos.GameObject
{
    [PacketHeader("u_as")]
    public class UseAOESkillPacket
    {
        #region Properties        

        [PacketIndex(0)]
        public int CastId { get; set; }

        [PacketIndex(1)]
        public short MapX { get; set; }

        [PacketIndex(2)]
        public short MapY { get; set; }

        #endregion
    }
}