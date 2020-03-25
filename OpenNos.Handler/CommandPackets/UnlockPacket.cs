using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;
using System.Linq;
using System.Threading.Tasks;

namespace OpenNos.Handler.CommandPackets
{
    [PacketHeader("$Pw", PassNonParseablePacket = true, Authority = AuthorityType.User)]
    public class UnlockPacket
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
                UnlockPacket packetDefinition = new UnlockPacket();
                if (!string.IsNullOrWhiteSpace(packetSplit[2]))
                {
                    packetDefinition._isParsed = true;
                    packetDefinition.Message = packetSplit[2];
                }
                packetDefinition.ExecuteHandler(sess);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(UnlockPacket), HandlePacket, ReturnHelp);

        public static string ReturnHelp() => "$SetPw Code";

        private void ExecuteHandler(ClientSession Session)
        {
            if (_isParsed)
            {
                if (Session.Character.SecondPassword != null)
                {
                    if (CryptographyBase.Sha512(p.Message) == Session.Character.SecondPassword)
                    {
                        Session.Character.hasVerifiedSecondPassword = true;
                        Session.SendPacket(Session.Character.GenerateSay($"You have successfully verified your identity!", 10));
                        Session.Character.HasGodMode = false;
                        Session.Character.InvisibleGm = false;
                        Session.Character.Invisible = false;
                        Session.SendPacket(Session.Character.GenerateInvisible());
                        Session.SendPacket(Session.Character.GenerateEq());
                        Session.SendPacket(Session.Character.GenerateCMode());
                        Session.Character.SideReputationAddBuff();
                        foreach (Mate teamMate in Session.Character.Mates.Where(m => m.IsTeamMember))
                        {
                            teamMate.PositionX = Session.Character.PositionX;
                            teamMate.PositionY = Session.Character.PositionY;
                            teamMate.UpdateBushFire();
                            Parallel.ForEach(Session.CurrentMapInstance.Sessions.Where(s => s.Character != null), s =>
                            {
                                if (ServerManager.Instance.ChannelId != 51 || Session.Character.Faction == s.Character.Faction)
                                {
                                    s.SendPacket(teamMate.GenerateIn(false, ServerManager.Instance.ChannelId == 51));
                                }
                                else
                                {
                                    s.SendPacket(teamMate.GenerateIn(true, ServerManager.Instance.ChannelId == 51, s.Account.Authority));
                                }
                            });
                            Session.SendPacket(Session.Character.GeneratePinit());
                            Session.Character.Mates.ForEach(s => Session.SendPacket(s.GenerateScPacket()));
                            Session.SendPackets(Session.Character.GeneratePst());
                        }
                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateIn(),
                            ReceiverType.AllExceptMe);
                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateGidx(),
                            ReceiverType.AllExceptMe);
                    }
                    else
                    {
                        Session.SendPacket(Session.Character.GenerateSay($"Wrong pin.", 10));
                    }
                }
                else
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You didn't set a pin yet. Use $SetPin to set a pin.", 10));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ReturnHelp(), 10));
            }
        }

        #endregion
    }
}