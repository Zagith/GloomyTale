﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core.Serializing;
using OpenNos.Domain;

namespace OpenNos.GameObject.Packets.CommandPackets
{
    [PacketHeader("$HairColor", PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.TGM, AuthorityType.TMOD } )]
    public class HairColorPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public HairColorType HairColor { get; set; }

        public static string ReturnHelp() => "$HairColor <Value>";

        #endregion
    }
}