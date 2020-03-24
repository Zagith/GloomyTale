using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$Pw", PassNonParseablePacket = true, Authorities = new AuthorityType[] { AuthorityType.User })]
    public class UnlockPacket
    {
        #region Properties

        [PacketIndex(0)]
        public string Message { get; set; }

        public static string ReturnHelp()
        {
            return "$Pw CODE";
        }

        #endregion
    }
}