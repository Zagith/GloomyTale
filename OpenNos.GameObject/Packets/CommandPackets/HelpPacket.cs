﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$Help", PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.TGM, AuthorityType.EventMaster } )]
    public class HelpPacket : PacketDefinition
    {
        [PacketIndex(0, SerializeToEnd = true)]
        public string Contents { get; set; }
    }
}