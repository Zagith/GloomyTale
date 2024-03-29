﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$CharEdit", PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.GA } )]
    public class CharacterEditPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string Property { get; set; }

        [PacketIndex(1, serializeToEnd: true)]
        public string Data { get; set; }

        public static string ReturnHelp() => "$CharEdit <Property> <Data>";

        #endregion
    }
}