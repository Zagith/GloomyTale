using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Handler.BasicPackets
{
    [PacketHeader("say")]
    public class SayPacket
    {
        #region Properties

        public string Message { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            string[] packetSplit = packet.Split(new[] { ' ' }, 3);
            if (packetSplit.Length < 3)
            {
                return;
            }
            SayPacket packetDefinition = new SayPacket();
            string msg = packetSplit[2].Trim();
            if (!string.IsNullOrWhiteSpace(msg))
            {
                packetDefinition.Message = msg;
                packetDefinition.ExecuteHandler(session as ClientSession);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(SayPacket), HandlePacket);

        private void ExecuteHandler(ClientSession Session)
        {
#warning TODO isAfk check
            //session.Character.IsAfk = false;
            /*Session.Character.MessageCounter += 2;
            if (Session.Character.MessageCounter > 11)
            {
                return;
            }*/

            var penalty = Session.Account.PenaltyLogs.OrderByDescending(s => s.DateEnd).FirstOrDefault();
            var message = Message;
            if (Session.Character.IsMuted() && penalty != null)
            {
                if (Session.Character.Gender == GenderType.Female)
                {
                    ConcurrentBag<ArenaTeamMember> member = ServerManager.Instance.ArenaTeams.ToList().FirstOrDefault(s => s.Any(e => e.Session == Session));
                    if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance && member != null)
                    {
                        var member2 = member.FirstOrDefault(o => o.Session == Session);
                        member.Replace(s => member2 != null && s.ArenaTeamType == member2.ArenaTeamType && s != member2).Replace(s => s.ArenaTeamType == member.FirstOrDefault(o => o.Session == Session)?.ArenaTeamType).ToList().ForEach(o => o.Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_FEMALE"), 1)));
                    }
                    else
                    {
                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_FEMALE"), 1));
                    }

                    Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"), (penalty.DateEnd - DateTime.Now).ToString(@"hh\:mm\:ss")), 11));
                    Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"), (penalty.DateEnd - DateTime.Now).ToString(@"hh\:mm\:ss")), 12));
                }
                else
                {
                    ConcurrentBag<ArenaTeamMember> member = ServerManager.Instance.ArenaTeams.ToList().FirstOrDefault(s => s.Any(e => e.Session == Session));
                    if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance && member != null)
                    {
                        var member2 = member.FirstOrDefault(o => o.Session == Session);
                        member.Replace(s => member2 != null && s.ArenaTeamType == member2.ArenaTeamType && s != member2).Replace(s => s.ArenaTeamType == member.FirstOrDefault(o => o.Session == Session)?.ArenaTeamType).ToList().ForEach(o => o.Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_MALE"), 1)));
                    }
                    else
                    {
                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_MALE"), 1));
                    }

                    Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"), (penalty.DateEnd - DateTime.Now).ToString(@"hh\:mm\:ss")), 11));
                    Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"), (penalty.DateEnd - DateTime.Now).ToString(@"hh\:mm\:ss")), 12));
                }
            }
            else
            {
                LogHelper.Instance.InsertChatLog(ChatType.General, Session.Character.CharacterId, message, Session.IpAddress);

                byte type = CharacterHelper.AuthorityChatColor(Session.Character.Authority);

                ConcurrentBag<ArenaTeamMember> member = null;
                lock (ServerManager.Instance.ArenaTeams)
                {
                    member = ServerManager.Instance.ArenaTeams.ToList().FirstOrDefault(s => s.Any(e => e.Session == Session));
                }
                if (Session.Character.Authority >= AuthorityType.GS)
                {
                    type = CharacterHelper.AuthorityChatColor(Session.Character.Authority);
                    if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance && member != null)
                    {
                        ArenaTeamMember member2 = member.FirstOrDefault(o => o.Session == Session);
                        member.Replace(s => member2 != null && s.ArenaTeamType == member2.ArenaTeamType && s != member2).Replace(s => s.ArenaTeamType == member.FirstOrDefault(o => o.Session == Session)?.ArenaTeamType).ToList().ForEach(o => o.Session.SendPacket(Session.Character.GenerateSay(message.Trim(), 1)));
                    }
                    else
                    {
                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateSay(message.Trim(), 1), ReceiverType.AllExceptMe);
                    }
                    message = $"[{Session.Character.Authority} {Session.Character.Name}]: " + message;
                }

                if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance && member != null)
                {
                    ArenaTeamMember member2 = member.FirstOrDefault(o => o.Session == Session);
                    member.Where(s => s.ArenaTeamType == member2?.ArenaTeamType && s != member2).ToList().ForEach(o => o.Session.SendPacket(Session.Character.GenerateSay(message.Trim(), type, Session.Account.Authority >= AuthorityType.GS)));
                }
                else if (ServerManager.Instance.ChannelId == 51 && Session.Account.Authority < AuthorityType.GM)
                {
                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateSay(message.Trim(), type, false), ReceiverType.AllExceptMeAct4);
                }
                else
                {
                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateSay(message.Trim(), type, Session.Character.Authority >= AuthorityType.GS), ReceiverType.AllExceptMe);
                }
            }
        }

        #endregion
    }
}
