using OpenNos.Core;
using OpenNos.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.GameObject.Packets.CommandPackets
{
    [PacketHeader("$ChannelStart", PassNonParseablePacket = true, Authorities = new AuthorityType[]{ AuthorityType.GA })]
    public class ChannelstartPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public byte Shout { get; set; }

        [PacketIndex(1)]
        public byte Corona { get; set; }

        public static string ReturnHelp()
        {
            return "$ChannelStart";
        }
    }
}
