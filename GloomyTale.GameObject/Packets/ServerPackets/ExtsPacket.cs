using OpenNos.Core;

namespace OpenNos.GameObject.Packets.ServerPackets
{
    [PacketHeader("exts")]
    public class ExtsPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public byte Type { get; set; }

        [PacketIndex(1)]
        public byte EquipmentExtension { get; set; }

        [PacketIndex(2)]
        public byte MainExtension { get; set; }

        [PacketIndex(3)]
        public byte EtcExtension { get; set; }
    }
}
