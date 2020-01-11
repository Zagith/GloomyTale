﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using GloomyTale.Core;
using GloomyTale.Domain;

namespace GloomyTale.GameObject.CommandPackets
{
    [PacketHeader("$Gold", PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.GA, AuthorityType.BA } )]
    public class GoldPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public long Amount { get; set; }

        public static string ReturnHelp() => "$Gold <Value>";

        #endregion
    }
}