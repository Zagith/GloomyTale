﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$ChangeClass", "$Class" , PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.TGM } )]
    public class ChangeClassPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public ClassType ClassType { get; set; }

        public static string ReturnHelp()
        {
            return "$ChangeClass | $Class <ClassType[0 = Adventurer, 1 = Swordsman, 2 = Archer, 3 = Mage, 4 = Martial Artist]>";
        }

        #endregion
    }
}
