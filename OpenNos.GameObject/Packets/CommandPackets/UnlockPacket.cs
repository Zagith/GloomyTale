using OpenNos.Core.Serializing;
using OpenNos.Domain;

namespace OpenNos.GameObject.Packets.CommandPackets
{
    [PacketHeader("$Pw", PassNonParseablePacket = true, Authorities = new AuthorityType[] { AuthorityType.User })]
    public class UnlockPacket : PacketDefinition
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