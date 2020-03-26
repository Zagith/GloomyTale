using OpenNos.Core;
using OpenNos.Core.Serializing;
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
    [PacketHeader("hero")]
    public class HeroPacket
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
            HeroPacket packetDefinition = new HeroPacket();
            if (!string.IsNullOrWhiteSpace(packetSplit[2]))
            {
                packetDefinition.Message = packetSplit[2].Trim();
                packetDefinition.ExecuteHandler(session as ClientSession);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(HeroPacket), HandlePacket);

        private void ExecuteHandler(ClientSession session)
        {
#warning TODO isAfk check
            //session.Character.IsAfk = false;
            if (session.Character.IsReputationHero() >= 3)
            {
                Message = Message.Trim();
                ServerManager.Instance.Broadcast(session, $"msg 5 [{session.Character.Name}]:{Message}",
                    ReceiverType.AllNoHeroBlocked);
            }
            else
            {
                session.SendPacket(
                    session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_HERO"), 11));
            }
        }

        #endregion
    }
}
