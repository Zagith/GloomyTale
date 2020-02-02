﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core.Serializing;
using OpenNos.Domain;

namespace OpenNos.GameObject.Packets.CommandPackets
{
    [PacketHeader("$GlobalEvent", PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.GA } )]
    public class GlobalEventPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public EventType EventType { get; set; }

        public static string ReturnHelp() => "$GlobalEvent <Type>";

        #endregion
    }
}