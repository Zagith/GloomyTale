using OpenNos.Core.Serializing;
using OpenNos.Domain;

namespace OpenNos.GameObject.Packets.CommandPackets
{
    [PacketHeader("$UserLog", PassNonParseablePacket = true, Authorities = new AuthorityType[] { AuthorityType.Administrator })]
    public class UserLogPacket : PacketDefinition
    {
        #region Methods

        public static string ReturnHelp() => "$UserLog";

        #endregion
    }
}