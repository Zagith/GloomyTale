using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Handler.BasicPackets
{
    [PacketHeader("/")]
    public class WhisperPacket
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
            WhisperPacket packetDefinition = new WhisperPacket();
            string msg = packetSplit[2].Trim();
            if (!string.IsNullOrWhiteSpace(msg))
            {
                packetDefinition.Message = msg;
                packetDefinition.ExecuteHandler(session as ClientSession);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(WhisperPacket), HandlePacket);

        private void ExecuteHandler(ClientSession Session)
        {
            //session.Character.IsAfk = false;
            try
            {
                // TODO: Implement WhisperSupport
                if (string.IsNullOrEmpty(Message))
                {
                    return;
                }

                string characterName =
                    Message.Split(' ')[
                            Message.StartsWith("GM ", StringComparison.CurrentCulture) ? 1 : 0].Replace("[Angel]", "").Replace("[Demon]", "");

                Enum.GetNames(typeof(AuthorityType)).ToList().ForEach(at => characterName = characterName.Replace($"[{at}]", ""));

                string message = "";
                string[] packetsplit = Message.Split(' ');
                for (int i = packetsplit[0] == "GM" ? 2 : 1; i < packetsplit.Length; i++)
                {
                    message += packetsplit[i] + " ";
                }

                if (message.Length > 60)
                {
                    message = message.Substring(0, 60);
                }

                message = message.Trim();
                Session.SendPacket(Session.Character.GenerateSpk(message, 5));
                CharacterDTO receiver = DAOFactory.CharacterDAO.LoadByName(characterName);
                int? sentChannelId = null;
                if (receiver != null)
                {
                    if (receiver.CharacterId == Session.Character.CharacterId)
                    {
                        return;
                    }

                    if (Session.Character.IsBlockedByCharacter(receiver.CharacterId))
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("BLACKLIST_BLOCKED")));
                        return;
                    }

                    ClientSession receiverSession =
                        ServerManager.Instance.GetSessionByCharacterId(receiver.CharacterId);
                    if (receiverSession?.CurrentMapInstance?.Map.MapId == Session.CurrentMapInstance?.Map.MapId
                        && Session.Account.Authority >= AuthorityType.GM)
                    {
                        receiverSession?.SendPacket(Session.Character.GenerateSay(message, 2));
                    }

                    sentChannelId = CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                    {
                        DestinationCharacterId = receiver.CharacterId,
                        SourceCharacterId = Session.Character.CharacterId,
                        SourceWorldId = ServerManager.Instance.WorldId,
                        Message = Session.Character.Authority >= AuthorityType.GS
                            ? Session.Character.GenerateSay(
                                $"(whisper)(From {Session.Character.Authority} {Session.Character.Name}):{message}", 11)
                            : Session.Character.GenerateSpk(message,
                                Session.Account.Authority >= AuthorityType.GM ? 15 : 5),
                        Type = Enum.GetNames(typeof(AuthorityType)).Any(a =>
                        {
                            if (a.Equals(packetsplit[0]))
                            {
                                Enum.TryParse(a, out AuthorityType auth);
                                if (auth >= AuthorityType.GM)
                                {
                                    return true;
                                }
                            }
                            return false;
                        })
                        || Session.Account.Authority >= AuthorityType.GM
                        ? MessageType.WhisperGM : MessageType.Whisper
                    });
                }

                if (sentChannelId == null)
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("USER_NOT_CONNECTED")));
                }
                else
                {
                    LogHelper.Instance.InsertChatLog(ChatType.Whisper, Session.Character.CharacterId, message, Session.IpAddress);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Whisper failed.", e);
            }
        }

        #endregion
    }
}
