using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.GameObject;
using OpenNos.Master.Library.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Handler.BasicPackets
{
    [PacketHeader("pulse")]
    public class PulsePacket
    {
        #region Properties

        public int Tick { get; set; }

        public bool IsAfk { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            string[] packetSplit = packet.Split(' ');
            if (packetSplit.Length < 4)
            {
                return;
            }
            PulsePacket packetDefinition = new PulsePacket();
            if (int.TryParse(packetSplit[2], out int tick))
            {
                packetDefinition.Tick = tick;
                packetDefinition.IsAfk = packetSplit[3] == "1";
                packetDefinition.ExecuteHandler(session as ClientSession);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(PulsePacket), HandlePacket);

        private void ExecuteHandler(ClientSession session)
        {
            if (session.Character.LastPulse.AddMilliseconds(80000) >= DateTime.UtcNow
                && DateTime.UtcNow >= session.Character.LastPulse.AddMilliseconds(40000))
            {
                session.Character.LastPulse = DateTime.UtcNow;
#warning TODO IsAfk check
                //session.Character.IsAfk = IsAfk;
                session.Character.MuteMessage();
                session.Character.DeleteTimeout();
                CommunicationServiceClient.Instance.PulseAccount(session.Account.AccountId);
            }
            else
            {
                session.Disconnect();
            }
        }

        #endregion
    }
}
