using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Handler.BasicPackets
{
    [PacketHeader("compl")]
    public class ComplimentPacket
    {
        #region Properties

        public long CharacterId { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            string[] packetSplit = packet.Split(' ');
            if (packetSplit.Length < 3)
            {
                return;
            }
            ComplimentPacket packetDefinition = new ComplimentPacket();
            if (long.TryParse(packetSplit[3], out long charId))
            {
                packetDefinition.CharacterId = charId;
                packetDefinition.ExecuteHandler(session as ClientSession);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(ComplimentPacket), HandlePacket);

        private void ExecuteHandler(ClientSession Session)
        {
            if (Session.Character.CharacterId == CharacterId)
            {
                return;
            }

            ClientSession sess = ServerManager.Instance.GetSessionByCharacterId(CharacterId);
            if (sess != null)
            {
                if (Session.Character.Level >= 30)
                {
                    GeneralLogDTO dto =
                        Session.Character.GeneralLogs.LastOrDefault(s =>
                            s.LogData == "World" && s.LogType == "Connection");
                    GeneralLogDTO lastcompliment =
                        Session.Character.GeneralLogs.LastOrDefault(s =>
                            s.LogData == "World" && s.LogType == "Compliment");
                    if (dto?.Timestamp.AddMinutes(60) <= DateTime.Now)
                    {
                        if (lastcompliment == null || lastcompliment.Timestamp.AddDays(1) <= DateTime.Now.Date)
                        {
                            sess.Character.Compliment++;
                            Session.SendPacket(Session.Character.GenerateSay(
                                string.Format(Language.Instance.GetMessageFromKey("COMPLIMENT_GIVEN"),
                                    sess.Character.Name), 12));
                            Session.Character.GeneralLogs.Add(new GeneralLogDTO
                            {
                                AccountId = Session.Account.AccountId,
                                CharacterId = Session.Character.CharacterId,
                                IpAddress = Session.IpAddress,
                                LogData = "World",
                                LogType = "Compliment",
                                Timestamp = DateTime.Now
                            });

                            Session.CurrentMapInstance?.Broadcast(Session,
                                Session.Character.GenerateSay(
                                    string.Format(Language.Instance.GetMessageFromKey("COMPLIMENT_RECEIVED"),
                                        Session.Character.Name), 12), ReceiverType.OnlySomeone,
                                characterId: CharacterId);
                        }
                        else
                        {
                            Session.SendPacket(
                                Session.Character.GenerateSay(
                                    Language.Instance.GetMessageFromKey("COMPLIMENT_COOLDOWN"), 11));
                        }
                    }
                    else if (dto != null)
                    {
                        Session.SendPacket(Session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("COMPLIMENT_LOGIN_COOLDOWN"),
                                (dto.Timestamp.AddMinutes(60) - DateTime.Now).Minutes), 11));
                    }
                }
                else
                {
                    Session.SendPacket(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("COMPLIMENT_NOT_MINLVL"),
                            11));
                }
            }
        }

        #endregion
    }
}
