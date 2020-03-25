using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.CommandPackets
{
    [PacketHeader("$Unstuck", PassNonParseablePacket = true, Authority = AuthorityType.User)]
    public class UnstuckPacket
    {
        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            if (session is ClientSession sess)
            {
                UnstuckPacket packetDefinition = new UnstuckPacket();
                packetDefinition.ExecuteHandler(sess);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(UnstuckPacket), HandlePacket, ReturnHelp);

        public static string ReturnHelp() => "$Unstuck ";

        private void ExecuteHandler(ClientSession session)
        {
            if (session.Character.Miniland == session.Character.MapInstance)
            {
                ServerManager.Instance.JoinMiniland(session, session);
            }
            else
            {
                ServerManager.Instance.ChangeMapInstance(session.Character.CharacterId,
                    session.Character.MapInstanceId, session.Character.PositionX, session.Character.PositionY,
                    true);
                session.SendPacket(StaticPacketHelper.Cancel(2));
            }
        }

        #endregion
    }
}