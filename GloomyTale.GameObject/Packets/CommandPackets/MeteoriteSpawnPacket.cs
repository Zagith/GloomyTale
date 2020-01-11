using GloomyTale.Core;
using GloomyTale.Domain;

namespace GloomyTale.GameObject.Packets.CommandPackets
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
