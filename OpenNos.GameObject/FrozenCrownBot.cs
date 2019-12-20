using GloomyTale.DiscordBot.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.GameObject
{
    public static class FrozenCrownBot
    {
        public static void RefreshAct4BotStat(int angel, int demon)
        {
            FrozenCrownExtension.Instance.AngelStat = angel;
            FrozenCrownExtension.Instance.DemonStat = demon;
        }
    }
}
