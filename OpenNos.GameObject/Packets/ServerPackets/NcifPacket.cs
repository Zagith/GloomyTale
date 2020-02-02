﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Core.Serializing;

namespace OpenNos.GameObject.Packets.ServerPackets
{
    [PacketHeader("ncif")]
    public class NcifPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte ObjectType { get; set; }

        [PacketIndex(1)]
        public long ObjectId { get; set; }

        #endregion
    }
}
