using GloomyTale.Core;
using GloomyTale.Domain;

namespace GloomyTale.GameObject.CommandPackets
{
    [PacketHeader("$UserLog", PassNonParseablePacket = true, Authorities = new AuthorityType[] { AuthorityType.Administrator })]
    public class UserLogPacket : PacketDefinition
    {
        #region Methods

        public static string ReturnHelp() => "$UserLog";

        #endregion
    }
}