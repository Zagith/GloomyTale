﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$ItemRain", PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.SGM } )]
    public class ItemRainPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public short VNum { get; set; }

        [PacketIndex(1)]
        public short Amount { get; set; }

        [PacketIndex(2)]
        public int Count { get; set; }

        [PacketIndex(3)]
        public int Time { get; set; }

        public static string ReturnHelp() => "$ItemRain <VNum> <Amount> <COUNT IN MAP> <Delay>";

        #endregion

    }
}