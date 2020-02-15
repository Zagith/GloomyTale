using GloomyTale.Core;
using GloomyTale.Domain;

namespace GloomyTale.GameObject.CommandPackets
{
    [PacketHeader("$PspXp", PassNonParseablePacket = true, Authorities = new AuthorityType[] { AuthorityType.TMOD })]
    public class PartnerSpXpPacket : PacketDefinition
    {
        #region Properties

        public static string ReturnHelp() => "$PspXp";

        #endregion
    }
}
