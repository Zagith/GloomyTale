﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core.Serializing;
using OpenNos.Domain;

namespace OpenNos.GameObject.Packets.CommandPackets
{
    [PacketHeader("$WigColor", PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.TGM, AuthorityType.TMOD } )]
    public class WigColorPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte Color { get; set; }

        public static string ReturnHelp() => "$WigColor <Value>";

        #endregion
    }
}