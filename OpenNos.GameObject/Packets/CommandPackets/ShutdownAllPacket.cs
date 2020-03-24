﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$ShutdownAll", PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.TM } )]
    public class ShutdownAllPacket
    {
        [PacketIndex(0)]
        public string WorldGroup { get; set; }

        public static string ReturnHelp() => "$ShutdownAll <WorldGroup|*>";
    }
}