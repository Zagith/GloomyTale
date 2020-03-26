﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using GloomyTale.Core;
using GloomyTale.Domain;

namespace GloomyTale.GameObject.CommandPackets
{
    [PacketHeader("$Backpack", PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.DEV } )]
    public class BackpackPacket : PacketDefinition
    {
        #region Methods

        public static string ReturnHelp() => "$Backpack";

        #endregion
    }
}