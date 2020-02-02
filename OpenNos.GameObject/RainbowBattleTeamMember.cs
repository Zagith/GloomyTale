using OpenNos.Domain;

namespace OpenNos.GameObject
{
    public class RainbowBattleTeamMember
    {
        public ClientSession Session { get; set; }
        public RainbowBattleTeamType RainbowBattleTeamType { get; set; }
        public bool Dead { get; set; }
        public bool Freezed { get; set; }

        public RainbowBattleTeamMember(ClientSession session, RainbowBattleTeamType rainbowbattleteamtype)
        {
            Session = session;
            RainbowBattleTeamType = rainbowbattleteamtype;
        }
    }
}
