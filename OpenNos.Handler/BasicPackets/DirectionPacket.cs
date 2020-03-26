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
    [PacketHeader("dir")]
    public class DirectionPacket
    {
        #region Properties

        public long CharacterId { get; set; }

        public byte Direction { get; set; }

        public int Option { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            string[] packetSplit = packet.Split(' ');
            if (packetSplit.Length < 5)
            {
                return;
            }
            DirectionPacket packetDefinition = new DirectionPacket();
            if (byte.TryParse(packetSplit[2], out byte dir)
                && int.TryParse(packetSplit[3], out int option)
                && long.TryParse(packetSplit[4], out long charId))
            {
                packetDefinition.Direction = dir;
                packetDefinition.Option = option;
                packetDefinition.CharacterId = charId;
                packetDefinition.ExecuteHandler(session as ClientSession);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(DirectionPacket), HandlePacket);

        private void ExecuteHandler(ClientSession session)
        {
            if (CharacterId == session.Character.CharacterId)
            {
                session.Character.Direction = Direction;
                session.CurrentMapInstance?.Broadcast(session.Character.GenerateDir());
            }
        }

        #endregion
    }
}
