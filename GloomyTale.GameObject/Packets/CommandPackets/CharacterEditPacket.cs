﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using GloomyTale.Core;
using GloomyTale.Domain;

namespace GloomyTale.GameObject.CommandPackets
{
    [PacketHeader("$CharEdit", PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.TM } )]
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