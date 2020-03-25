using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.CommandPackets.Character
{
    [PacketHeader("$UserLog", PassNonParseablePacket = true, Authority = AuthorityType.GA)]
    public class UserLogPacket
    {
        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            if (session is ClientSession sess)
            {
                UserLogPacket packetDefinition = new UserLogPacket();
                packetDefinition.ExecuteHandler(sess);
                LogHelper.Instance.InsertCommandLog(sess.Character.CharacterId, packet, sess.IpAddress);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(UserLogPacket), HandlePacket, ReturnHelp);

        public static string ReturnHelp() => "$Undercover ";

        private void ExecuteHandler(ClientSession session)
        {
            int n = 1;
            foreach (string username in ClientSession.UserLog)
            {
                session.SendPacket(session.Character.GenerateSay($"{n++}- {username}", 12));
            }

            session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
        }

        #endregion
    }
}