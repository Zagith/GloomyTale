﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;

namespace OpenNos.GameObject
{
    [PacketHeader("pcl")]
    public class PclPacket
    {
        #region Properties

        [PacketIndex(0)]
        public int Type { get; set; }

        [PacketIndex(1)]
        public int Unknown { get; set; }

        #endregion
    }
}