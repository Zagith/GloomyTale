using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.BasicPackets
{
    [PacketHeader(";")]
    public class GroupTalkPacket
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
            GroupTalkPacket packetDefinition = new GroupTalkPacket();
            if (!string.IsNullOrWhiteSpace(packetSplit[2]))
            {
                packetDefinition.Message = packetSplit[2];
                packetDefinition.ExecuteHandler(session as ClientSession);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(GroupTalkPacket), HandlePacket);

        private void ExecuteHandler(ClientSession session)
        {
#warning TODO isAfk check
            //session.Character.IsAfk = false;

            LogHelper.Instance.InsertChatLog(ChatType.Party, session.Character.CharacterId, Message, session.IpAddress);
            ServerManager.Instance.Broadcast(session, session.Character.GenerateSpk(Message, 3), ReceiverType.Group);
        }

        #endregion
    }
}
