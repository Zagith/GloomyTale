using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.Packets.CommandPackets
{
    [PacketHeader("$Meteor", PassNonParseablePacket = true, Authorities = new AuthorityType[] { AuthorityType.GM })]
    public class MeteoriteSpawnPacket : PacketDefinition
    {
        public static string ReturnHelp()
        {
            return "$Meteor";
        }
    }
}
