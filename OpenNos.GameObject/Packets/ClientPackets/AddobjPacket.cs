﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;

namespace OpenNos.GameObject
{
    [PacketHeader("addobj")]
    public class AddObjPacket
    {
        #region Properties


        [PacketIndex(0)]
        public short Slot { get; set; }
        [PacketIndex(1)]
        public short PositionX { get; set; }
        [PacketIndex(2)]
        public short PositionY { get; set; }
        #endregion
    }
}