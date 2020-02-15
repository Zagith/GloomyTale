using OpenNos.Master.Library.Client;

namespace OpenNos.GameObject
{
    public static class FrozenCrownBot
    {
        public static void RefreshAct4BotStat(int angel, int demon)
        {
            DiscordServiceClient.Instance.RefreshAct4Stat(angel, demon);
        }
    }
}
