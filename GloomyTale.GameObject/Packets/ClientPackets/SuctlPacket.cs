using GloomyTale.Core;
using GloomyTale.Domain;

namespace GloomyTale.GameObject
{
    [PacketHeader("suctl")]
    public class SuctlPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public int CastId { get; set; }

        [PacketIndex(1)]
        public int Unknown2 { get; set; }

        [PacketIndex(2)]
        public int MateTransportId { get; set; }

        [PacketIndex(3)]
        public VisualType TargetType { get; set; }

        [PacketIndex(4)]
        public long TargetId { get; set; }
    }
}
