﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$JLvl", PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.TGM } )]
    public class ChangeJobLevelPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte JobLevel { get; set; }

        public static string ReturnHelp() => "$JLvl <Value>";

        #endregion
    }
}