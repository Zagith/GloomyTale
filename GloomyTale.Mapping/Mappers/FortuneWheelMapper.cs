using GloomyTale.DAL.EF.Entities;
using OpenNos.Data;

namespace OpenNos.Mapper.Mappers
{
    public static class FortuneWheelMapper
    {
        #region Methods

        public static bool ToFortuneWheel(FortuneWheelDTO input, FortuneWheel output)
        {
            if (input == null)
            {
                return false;
            }

            output.IsRareRandom = input.IsRareRandom;
            output.Rare = input.Rare;
            output.Upgrade = input.Rare;
            output.ItemGeneratedAmount = input.ItemGeneratedAmount;
            output.ItemGeneratedVNum = input.ItemGeneratedVNum;
            output.Probability = input.Probability;
            output.ShopId = input.ShopId;

            return true;
        }

        public static bool ToFortuneWheelDTO(FortuneWheel input, FortuneWheelDTO output)
        {
            if (input == null)
            {
                return false;
            }

            output.IsRareRandom = input.IsRareRandom;
            output.Rare = input.Rare;
            output.Upgrade = input.Rare;
            output.ItemGeneratedAmount = input.ItemGeneratedAmount;
            output.ItemGeneratedVNum = input.ItemGeneratedVNum;
            output.Probability = input.Probability;
            output.ShopId = input.ShopId;

            return true;
        }

        #endregion
    }
}
