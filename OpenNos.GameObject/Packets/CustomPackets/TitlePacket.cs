using OpenNos.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.GameObject.Packets.CustomPackets
{
    [PacketHeader("title")]
    public class TitlePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string TitleData { get; set; }

        #endregion
    }
}
