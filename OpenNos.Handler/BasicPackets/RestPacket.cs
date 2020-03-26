using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.GameObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Handler.BasicPackets
{
    [PacketHeader("rest")]
    public class RestPacket
    {
        #region Properties

        public byte Amount { get; set; }

        public List<Tuple<byte, long>> Users { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            string[] packetSplit = packet.Split(' ');
            if (packetSplit.Length < 4)
            {
                return;
            }
            RestPacket packetDefinition = new RestPacket();
            if (byte.TryParse(packetSplit[2], out byte amount))
            {
                packetDefinition.Users = new List<Tuple<byte, long>>();
                for (int i = 3; i < packetSplit.Length - 1; i += 2)
                {
                    if (byte.TryParse(packetSplit[i], out byte userType) && long.TryParse(packetSplit[i + 1], out long userId))
                    {
                        packetDefinition.Users.Add(new Tuple<byte, long>(userType, userId));
                    }
                }
                packetDefinition.Amount = amount;
                packetDefinition.ExecuteHandler(session as ClientSession);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(RestPacket), HandlePacket);

        private void ExecuteHandler(ClientSession session)
        {
#warning TODO isAfk check
            //session.Character.IsAfk = false;
            if (session.Character.MeditationDictionary.Count != 0)
            {
                session.Character.MeditationDictionary.Clear();
            }
            foreach (Tuple<byte, long> user in Users)
            {
                if (user.Item1 == 1)
                {
                    session.Character.Rest();
                }
                else
                {
                    session.CurrentMapInstance.Broadcast(session.Character.Mates
                        .Find(s => s.MateTransportId == (int)user.Item2)?.GenerateRest(Users[0] != user));
                }
            }
        }

        #endregion
    }
}
