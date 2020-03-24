﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$Warn", PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.GS, AuthorityType.TGM, AuthorityType.TMOD } )]
    public class WarningPacket
    {
        #region Properties

        [PacketIndex(0)]
        public string CharacterName { get; set; }

        [PacketIndex(1, serializeToEnd: true)]
        public string Reason { get; set; }

        public static string ReturnHelp() => "$Warn <Nickname> <Reason>";

        #endregion
    }
}