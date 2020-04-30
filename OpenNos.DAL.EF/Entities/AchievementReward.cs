using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.EF.Entities
{
    public class AchievementReward
    {
        #region Properties

        public long AchievementRewardId { get; set; }

        public byte RewardType { get; set; }

        public int Data { get; set; }

        public byte Design { get; set; }

        public byte Rarity { get; set; }

        public byte Upgrade { get; set; }

        public int Amount { get; set; }

        public long AchievementId { get; set; }

        #endregion
    }
}
