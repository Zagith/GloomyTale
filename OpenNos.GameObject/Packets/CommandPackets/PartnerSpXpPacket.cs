using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$PspXp", PassNonParseablePacket = true, Authorities = new AuthorityType[] { AuthorityType.TMOD })]
    public class PartnerSpXpPacket
    {
        #region Properties

        public static string ReturnHelp() => "$PspXp";

        #endregion
    }
}
