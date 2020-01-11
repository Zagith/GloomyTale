﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using GloomyTale.Core;
using GloomyTale.Domain;

namespace GloomyTale.GameObject.CommandPackets
{
    [PacketHeader("$AddMonster" , PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.TM } )]
    public class AddMonsterPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public short MonsterVNum { get; set; }

        [PacketIndex(1)]
        public bool IsMoving { get; set; }

        public static string ReturnHelp() => "$AddMonster <VNum> <IsMoving>";

        #endregion
    }
}