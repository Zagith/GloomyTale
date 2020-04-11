using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$CH", PassNonParseablePacket = true, Authorities = new AuthorityType[] { AuthorityType.SGM })] //for the momment
    public class CHPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int ch { get; set; }

        #endregion

        #region Methods

        public static string ReturnHelp() => "$CH <CH Id>";

        #endregion
    }
}
