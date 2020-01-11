﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using GloomyTale.Core;
using GloomyTale.Domain;

namespace GloomyTale.GameObject.CommandPackets
{
    [PacketHeader("$Drop", PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.GA, AuthorityType.BA  } )]
    public class DropPacket : PacketDefinition
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

        public static string ReturnHelp() => "$Drop <VNum> <Amount> <Count> <Delay>";

        #endregion

    }
}