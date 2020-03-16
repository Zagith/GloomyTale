﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$SearchMonster", "$SMonster", PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.GA, AuthorityType.BA } )]
    public class SearchMonsterPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0, SerializeToEnd = true)]
        public string Contents { get; set; }

        public static string ReturnHelp() => "$SearchMonster <Name>";

        #endregion
    }
}