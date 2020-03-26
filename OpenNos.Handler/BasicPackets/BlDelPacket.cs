using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.BasicPackets
{
    [PacketHeader("bldel")]
    public class BlDelPacket
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
            BlDelPacket packetDefinition = new BlDelPacket();
            if (long.TryParse(packetSplit[2], out long charId))
            {
                packetDefinition.CharacterId = charId;
                packetDefinition.ExecuteHandler(session as ClientSession);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(BlDelPacket), HandlePacket);

        private void ExecuteHandler(ClientSession session)
        {
            session.Character.DeleteBlackList(CharacterId);
            session.SendPacket(session.Character.GenerateBlinit());
            session.SendPacket(
                UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("BLACKLIST_DELETED")));
        }

        #endregion
    }
}
