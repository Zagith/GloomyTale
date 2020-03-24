using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$Unstuck", PassNonParseablePacket = true, Authorities = new AuthorityType[] { AuthorityType.User })]
    public class UnstuckPacket
    {
        #region Methods

        public static string ReturnHelp() => "$Unstuck";

        #endregion
    }
}