﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$GodMode", PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.TGM, AuthorityType.EventMaster } )]
    public class GodModePacket : PacketDefinition
    {
        public static string ReturnHelp() => "$GodMode";
    }
}