﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$Stat", PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.TGM } )]
    public class StatCommandPacket : PacketDefinition
    {
        public static string ReturnHelp() => "$Stat";
    }
}