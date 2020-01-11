﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using GloomyTale.Core;
using GloomyTale.Domain;

namespace GloomyTale.GameObject.CommandPackets
{
    [PacketHeader("$HelpMe", PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.TM } )]
    public class HelpMePacket : PacketDefinition
    {

        [PacketIndex(0, SerializeToEnd = true)]
        public string Message { get; set; }

        #region Methods

        public static string ReturnHelp() => "$HelpMe <Message>";

        #endregion
    }
}