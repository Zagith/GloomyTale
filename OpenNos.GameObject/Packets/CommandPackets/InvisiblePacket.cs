﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$Invisible" , PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.TGM, AuthorityType.TMOD, AuthorityType.EventMaster } )]
    public class InvisiblePacket : PacketDefinition
    {
        public static string ReturnHelp() => "$Invisible";
    }
}