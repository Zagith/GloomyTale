﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$BlockRep", PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.SGM } )]
    public class BlockRepPacket
    {
        #region Properties

        [PacketIndex(0)]
        public string CharacterName { get; set; }

        [PacketIndex(1)]
        public int Duration { get; set; }

        [PacketIndex(2, SerializeToEnd = true)]
        public string Reason { get; set; }

        public static string ReturnHelp() => "$BlockRep <Nickname> <Duration> <Reason>";

        #endregion

    }
}