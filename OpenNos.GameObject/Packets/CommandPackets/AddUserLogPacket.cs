using OpenNos.Core.Serializing;
using OpenNos.Domain;

namespace OpenNos.GameObject.Packets.CommandPackets
{
    [PacketHeader("$AddUserLog", PassNonParseablePacket = true, Authorities = new AuthorityType[] { AuthorityType.Administrator })]
    public class AddUserLogPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string Username { get; set; }

        #endregion

        #region Methods

        public static string ReturnHelp() => "$AddUserLog <Username>";

        #endregion
    }
}
