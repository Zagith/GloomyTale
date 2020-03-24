﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$AddNpc" , PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.TM } )]
    public class AddNpcPacket
    {
        #region Properties

        [PacketIndex(0)]
        public short NpcVNum { get; set; }

        [PacketIndex(1)]
        public bool IsMoving { get; set; }

        public static string ReturnHelp() => "$AddNpc <VNum> <IsMoving>";

        #endregion
    }
}