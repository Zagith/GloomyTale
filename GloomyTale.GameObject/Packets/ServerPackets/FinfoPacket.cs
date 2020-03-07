using GloomyTale.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.GameObject.Packets.ServerPackets
{
    [PacketHeader("finfo")]
    public class FinfoPacket : PacketDefinition
    {

        [PacketIndex(0)]
        public List<FinfoSubPackets> FriendList { get; set; }
    }
}
