using OpenNos.Domain;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject
{
    public class BandicootMember
    {
        public ClientSession Session { get; set; }
        public EventType BandicootRunType { get; set; }
    }
}
