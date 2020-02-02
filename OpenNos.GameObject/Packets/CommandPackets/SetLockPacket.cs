using OpenNos.Core.Serializing;
using OpenNos.Domain;

namespace OpenNos.GameObject.Packets.CommandPackets
{
    [PacketHeader("$SetPw", PassNonParseablePacket = true, Authorities = new AuthorityType[] { AuthorityType.User })]
    public class SetLockPacket : PacketDefinition
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