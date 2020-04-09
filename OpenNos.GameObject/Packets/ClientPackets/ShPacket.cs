using OpenNos.Core;
using OpenNos.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.GameObject.Packets.ClientPackets
{
    [PacketHeader("sh")]
    public class ShPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public UserType TargetType { get; set; } //Not sure, need to verify

        [PacketIndex(1)]
        public int TargetId { get; set; }
    }
}
