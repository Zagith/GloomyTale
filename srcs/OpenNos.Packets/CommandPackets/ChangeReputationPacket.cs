﻿// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$ChangeRep", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class ChangeReputationPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public long Reputation { get; set; }

        public static string ReturnHelp() => "$ChangeRep AMOUNT";

        #endregion
    }
}