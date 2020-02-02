﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core.Serializing;
using OpenNos.Domain;

namespace OpenNos.GameObject.Packets.CommandPackets
{
    [PacketHeader("$HairStyle", PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.TGM, AuthorityType.TMOD } )]
    public class HairStylePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public HairStyleType HairStyle { get; set; }

        public static string ReturnHelp() => "$HairStyle <Value>";

        #endregion
    }
}