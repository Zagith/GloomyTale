﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$Act4Stat", PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.SGM } )]
    public class Act4StatPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte Faction { get; set; }

        [PacketIndex(1)]
        public int Value { get; set; }

        public static string ReturnHelp() => "$Act4Stat <Faction> <Value>";

        #endregion
    }
}