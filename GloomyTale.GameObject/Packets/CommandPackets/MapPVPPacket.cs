﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using GloomyTale.Core;
using GloomyTale.Domain;

namespace GloomyTale.GameObject.CommandPackets
{
    [PacketHeader("$MapPVP" , PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.GA, AuthorityType.BA } )]
    public class MapPVPPacket : PacketDefinition
    {
        public static string ReturnHelp() => "$MapPVP";
    }
}