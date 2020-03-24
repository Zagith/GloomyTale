﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$Promote", PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.GA } )]
    public class PromotePacket
    {
        #region Properties

        [PacketIndex(0)]
        public string CharacterName { get; set; }

        public static string ReturnHelp() => "$Promote <Nickname>";

        #endregion
    }
}