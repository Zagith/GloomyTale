using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.CommandPackets.Character
{
    [PacketHeader("$AddUserLog", PassNonParseablePacket = true, Authority = AuthorityType.Administrator)]
    public class AddUserLogPacket
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
            AddUserLogPacket packetDefinition = new AddUserLogPacket();
            if (!string.IsNullOrWhiteSpace(packetSplit[2]))
            {
                packetDefinition._isParsed = true;
                packetDefinition.Username = packetSplit[2];
            }
            packetDefinition.ExecuteHandler(sess);
            LogHelper.Instance.InsertCommandLog(sess.Character.CharacterId, packet, sess.IpAddress);
        }

        public static void Register() => PacketFacility.AddHandler(typeof(AddUserLogPacket), HandlePacket, ReturnHelp);

        public static string ReturnHelp() => "$AddUserLog Username";

        private void ExecuteHandler(ClientSession session)
        {
            if (_isParsed)
            {
                ClientSession.UserLog.Add(Username);
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
