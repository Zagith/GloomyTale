using OpenNos.Core.Serializing;
using OpenNos.Domain;

namespace OpenNos.GameObject.Packets.CommandPackets
{
    [PacketHeader("$PspXp", PassNonParseablePacket = true, Authorities = new AuthorityType[] { AuthorityType.TMOD })]
    public class PartnerSpXpPacket : PacketDefinition
    {
        #region Properties

        public static string ReturnHelp() => "$PspXp";

        #endregion
    }
}
