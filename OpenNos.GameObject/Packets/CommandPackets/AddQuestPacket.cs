﻿using OpenNos.Core.Serializing;
using OpenNos.Domain;

namespace OpenNos.GameObject.Packets.CommandPackets
{
    [PacketHeader("$AddQuest", PassNonParseablePacket = true, Authorities = new AuthorityType[] { AuthorityType.GA })]
    public class AddQuestPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public short QuestId { get; set; }

        public static string ReturnHelp() => "$AddQuest <QuestId>";

        #endregion
    }
}
