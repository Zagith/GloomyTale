using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$SetPw", PassNonParseablePacket = true, Authorities = new AuthorityType[] { AuthorityType.User })]
    public class SetLockPacket
    {
        #region Properties

        [PacketIndex(0)]
        public string Message { get; set; }

        public static string ReturnHelp()
        {
            return "$SetPw CODE";
        }

        #endregion
    }
}