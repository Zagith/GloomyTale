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

namespace OpenNos.Handler.BasicPackets
{
    [PacketHeader("btk")]
    public class FriendBTalkPacket
    {
        #region Properties

        public long CharacterId { get; set; }

        public string Message { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            string[] packetSplit = packet.Split(new[] { ' ' }, 4);
            if (packetSplit.Length < 4)
            {
                return;
            }
            FriendBTalkPacket packetDefinition = new FriendBTalkPacket();
            string msg = packetSplit[3].Trim();
            if (long.TryParse(packetSplit[2], out long charId) && !string.IsNullOrWhiteSpace(msg))
            {
                packetDefinition.CharacterId = charId;
                packetDefinition.Message = msg;
                packetDefinition.ExecuteHandler(session as ClientSession);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(FriendBTalkPacket), HandlePacket);

        private void ExecuteHandler(ClientSession session)
        {
#warning TODO IsAfk check
            //session.Character.IsAfk = false;
            string message = Message;
            if (message.Length > 60)
            {
                message = message.Substring(0, 60);
            }

            message = message.Trim();

            CharacterDTO character = DAOFactory.CharacterDAO.LoadById(CharacterId);
            if (character != null)
            {
                int? sentChannelId = CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                {
                    DestinationCharacterId = character.CharacterId,
                    SourceCharacterId = session.Character.CharacterId,
                    SourceWorldId = ServerManager.Instance.WorldId,
                    Message = session.Character.GenerateTalk(message),
                    Type = MessageType.PrivateChat
                });

                LogHelper.Instance.InsertChatLog(ChatType.Friend, session.Character.CharacterId, Message, session.IpAddress);

                if (!sentChannelId.HasValue) //character is even offline on different world
                {
                    session.SendPacket(
                        UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("FRIEND_OFFLINE")));
                }
            }
        }

        #endregion
    }
}
