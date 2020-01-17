﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using GloomyTale.Core;
using GloomyTale.Domain;

namespace GloomyTale.GameObject.CommandPackets
{
    [PacketHeader("$ChangeDignity", "$Dignity" , PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.TGM, AuthorityType.TMOD } )]
    public class ChangeDignityPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public float Dignity { get; set; }

        public static string ReturnHelp() => "$ChangeDignity | $Dignity <Value>";

        #endregion
    }
}