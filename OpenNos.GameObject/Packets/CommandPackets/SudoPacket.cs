﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core.Serializing;
using OpenNos.Domain;

namespace OpenNos.GameObject.Packets.CommandPackets
{
    [PacketHeader("$Sudo", PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.TM } )]
    public class SudoPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public string CharacterName { get; set; }

        [PacketIndex(1, serializeToEnd: true)]
        public string CommandContents { get; set; }

        public static string ReturnHelp() => "$Sudo <Nickname> <Command>";
    }
}