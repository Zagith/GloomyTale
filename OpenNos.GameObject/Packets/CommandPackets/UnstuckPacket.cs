using OpenNos.Core.Serializing;
using OpenNos.Domain;

namespace OpenNos.GameObject.Packets.CommandPackets
{
    [PacketHeader("$Unstuck", PassNonParseablePacket = true, Authorities = new AuthorityType[] { AuthorityType.User })]
    public class UnstuckPacket : PacketDefinition
    {
        #region Methods

        public static string ReturnHelp() => "$Unstuck";

        #endregion
    }
}