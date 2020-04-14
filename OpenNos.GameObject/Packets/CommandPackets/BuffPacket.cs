﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$Buff", PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.TGM } )]
    public class BuffPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public short CardId { get; set; }

        [PacketIndex(1)]
        public byte? Level { get; set; }

        #endregion

        #region Methods

        public static string ReturnHelp() => "$Buff <CardId> <?Level>";

        #endregion
    }
}