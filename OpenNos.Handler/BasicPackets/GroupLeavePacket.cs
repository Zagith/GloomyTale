using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Handler.BasicPackets
{
    [PacketHeader("pleave")]
    public class GroupLeavePacket
    {
        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            GroupLeavePacket packetDefinition = new GroupLeavePacket();
            packetDefinition.ExecuteHandler(session as ClientSession);
        }

        public static void Register() => PacketFacility.AddHandler(typeof(GroupLeavePacket), HandlePacket);

        private void ExecuteHandler(ClientSession session) => ServerManager.Instance.GroupLeave(session);

        #endregion
    }
}
