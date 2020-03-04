using GloomyTale.Core;

namespace GloomyTale.GameObject.Packets.ServerPackets
{
    [PacketHeader("cl")]
    public class ClPacket : PacketDefinition
    {

        [PacketIndex(0)]
        public long VisualId { get; set; }
        [PacketIndex(1)]
        public bool Camouflage { get; set; }
        [PacketIndex(2)]
        public bool Invisible { get; set; }
    }
}
