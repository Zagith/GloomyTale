using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Domain;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject.Packets.CommandPackets
{
    [PacketHeader("$Meteor", PassNonParseablePacket = true, Authority = AuthorityType.SGM)]
    public class MeteoriteSpawnPacket
    {
        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            if (session is ClientSession sess)
            {
                MeteoriteSpawnPacket packetDefinition = new MeteoriteSpawnPacket();
                packetDefinition.ExecuteHandler(sess);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(MeteoriteSpawnPacket), HandlePacket, ReturnHelp);

        public static string ReturnHelp() => "$Invisible";

        private void ExecuteHandler(ClientSession session)
        {
            Logger.LogUserEvent("GMCOMMAND", session.GenerateIdentity(), $"[MeteoriteSpawn]");

            ServerManager.Instance.MeteoriteSpawn();
        }

        #endregion
    }
}
