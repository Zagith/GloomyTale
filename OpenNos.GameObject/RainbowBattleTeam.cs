using OpenNos.Domain;
using System.Collections.Generic;

namespace OpenNos.GameObject
{
    public class RainbowBattleTeam
    {
        public List<RainbowBattleMember> Member { get; set; }
        public List<ClientSession> Session { get; set; }
        public int Points { get; set; }
        public RainbowBattleTeamType Color { get; set; }
        public EventType RainbowBattleType { get; set; }
    }
}
