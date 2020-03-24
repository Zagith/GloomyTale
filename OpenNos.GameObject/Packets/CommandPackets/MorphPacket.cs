﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$Morph", PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.SGM, AuthorityType.SMOD } )]
    public class MorphPacket
    {
        #region Properties

        [PacketIndex(0)]
        public int MorphId { get; set; }

        [PacketIndex(1)]
        public byte Upgrade { get; set; }

        [PacketIndex(2)]
        public byte MorphDesign { get; set; }

        [PacketIndex(3)]
        public int ArenaWinner { get; set; }

        public static string ReturnHelp()
        {
            return "$Morph <VNum> <Upgrade> <Wings> <IsArenaWinner>";
        }

        #endregion
    }
}