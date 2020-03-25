using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.CommandPackets
{
    [PacketHeader("$RemoveUserLog", PassNonParseablePacket = true, Authority = AuthorityType.Administrator)]
    public class RemoveUserLogPacket
    {
        #region Members

        private bool _isParsed;

        #endregion

        #region Properties

        public string Username { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            string[] packetSplit = packet.Split(' ');
            if (!(session is ClientSession sess))
            {
                return;
            }
            if (packetSplit.Length < 3)
            {
                sess.SendPacket(sess.Character.GenerateSay(ReturnHelp(), 10));
                return;
            }
            RemoveUserLogPacket packetDefinition = new RemoveUserLogPacket();
            if (!string.IsNullOrWhiteSpace(packetSplit[2]))
            {
                packetDefinition._isParsed = true;
                packetDefinition.Username = packetSplit[2];
            }
            packetDefinition.ExecuteHandler(sess);
            LogHelper.Instance.InsertCommandLog(sess.Character.CharacterId, packet, sess.IpAddress);
        }

        public static void Register() => PacketFacility.AddHandler(typeof(RemoveUserLogPacket), HandlePacket, ReturnHelp);

        public static string ReturnHelp() => "$RemoveUserLog Username";

        private void ExecuteHandler(ClientSession session)
        {
            if (_isParsed)
            {
                if (ClientSession.UserLog.Contains(Username))
                {
                    ClientSession.UserLog.RemoveAll(username => username == Username);
                }

                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
            }
            else
            {
                session.SendPacket(session.Character.GenerateSay(ReturnHelp(), 10));
            }
        }

        #endregion
    }
}