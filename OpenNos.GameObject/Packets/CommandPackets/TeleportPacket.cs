﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core.Serializing;
using OpenNos.Domain;

namespace OpenNos.GameObject.Packets.CommandPackets
{
    [PacketHeader("$Teleport" , PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.TGM, AuthorityType.BA, AuthorityType.GS } )]
    public class TeleportPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string Data { get; set; }

        [PacketIndex(1)]
        public short X { get; set; }

        [PacketIndex(2)]
        public short Y { get; set; }

        public static string ReturnHelp() => "$Teleport <Nickname|ToMapId> <?ToX> <?ToY>";

        #endregion
    }
}