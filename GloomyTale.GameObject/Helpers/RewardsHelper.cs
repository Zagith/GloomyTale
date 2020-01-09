using OpenNos.Core;

namespace OpenNos.GameObject.Helpers
{
    public class RewardsHelper
    {
        #region Methods

        public static int ArenaXpReward(byte characterLevel)
        {
            if (characterLevel <= 39)
            {
                // 25%
                return (int)(CharacterHelper.XPData[characterLevel] / 4);
            }

            if (characterLevel <= 55)
            {
                // 20%
                return (int)(CharacterHelper.XPData[characterLevel] / 5);
            }

            if (characterLevel <= 75)
            {
                // 10%
                return (int)(CharacterHelper.XPData[characterLevel] / 10);
            }

            if (characterLevel <= 79)
            {
                // 5%
                return (int)(CharacterHelper.XPData[characterLevel] / 20);
            }

            if (characterLevel <= 85)
            {
                // 2%
                return (int)(CharacterHelper.XPData[characterLevel] / 50);
            }

            if (characterLevel <= 90)
            {
                return (int)(CharacterHelper.XPData[characterLevel] / 80);
            }

            if (characterLevel <= 93)
            {
                return (int)(CharacterHelper.XPData[characterLevel] / 100);
            }

            if (characterLevel <= 99)
            {
                return (int)(CharacterHelper.XPData[characterLevel] / 1000);
            }

            return 0;
        }

        #endregion
    }
}