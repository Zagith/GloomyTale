using OpenNos.Domain;

namespace OpenNos.GameObject
{
    public class RainbowBattleMember
    {
        public ClientSession Session { get; set; }
        public long? GroupId { get; set; }
        public EventType RainbowBattleType { get; set; }
    }
}
