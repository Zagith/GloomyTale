using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloomyTale.DiscordBot.Extensions
{
    public class FrozenCrownExtension
    {
        private static FrozenCrownExtension _instance;

        public static FrozenCrownExtension Instance => _instance ?? (_instance = new FrozenCrownExtension());
        public int AngelStat { get; set; }

        public int DemonStat { get; set; }

        public int SendAct4Stat(int faction)
        {
            switch (faction)
            {
                case 1:
                    return AngelStat;
                case 2:
                    return DemonStat;
            }
            return 0;
        }
    }
}
