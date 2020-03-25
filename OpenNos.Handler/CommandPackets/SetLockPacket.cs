using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Domain;
using OpenNos.GameObject;

namespace OpenNos.Handler.CommandPackets
{
    [PacketHeader("$SetPw", PassNonParseablePacket = true, Authority = AuthorityType.User)]
    public class SetLockPacket
    {
        #region Members

        private bool _isParsed;

        #endregion

        #region Properties

        public string Message { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            if (session is ClientSession sess)
            {
                string[] packetSplit = packet.Split(new[] { ' ' }, 3);
                if (packetSplit.Length < 3)
                {
                    sess.SendPacket(sess.Character.GenerateSay(ReturnHelp(), 10));
                    return;
                }
                SetLockPacket packetDefinition = new SetLockPacket();
                if (!string.IsNullOrWhiteSpace(packetSplit[2]))
                {
                    packetDefinition._isParsed = true;
                    packetDefinition.Message = packetSplit[2];
                }
                packetDefinition.ExecuteHandler(sess);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(SetLockPacket), HandlePacket, ReturnHelp);

        public static string ReturnHelp() => "$SetPw Code";

        private void ExecuteHandler(ClientSession session)
        {
            if (_isParsed)
            {
                if (session.Character.SecondPassword == null)
                {
                    if (Message.Length >= 8)
                    {
                        session.Character.SecondPassword = CryptographyBase.Sha512(Message);
                        session.Character.Save();
                        session.Character.hasVerifiedSecondPassword = true;
                        session.SendPacket(session.Character.GenerateSay($"Done! Your second password (or pin) is now: {Message}. Do not forget it.", 10));
                        session.Character.HasGodMode = false;
                    }
                    else
                    {
                        session.SendPacket(session.Character.GenerateSay($"Your pin lenght cannot be less than 8 characters.", 10));
                    }
                }
                else
                {
                    session.SendPacket(session.Character.GenerateSay($"You already have a pin. Please, if you have forgotten it, contact a staff member.", 10));
                }
            }
            else
            {
                session.SendPacket(session.Character.GenerateSay(ReturnHelp(), 10));
            }
        }

        #endregion
    }
}