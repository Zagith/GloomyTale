﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$Shout", PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.SGM, AuthorityType.SMOD } )]
    public class ShoutPacket
    {
        #region Properties

        [PacketIndex(0, SerializeToEnd = true)]
        public string Message { get; set; }

        public static string ReturnHelp() => "$Shout <Message>";

        #endregion
    }
}