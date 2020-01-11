using GloomyTale.DAL.EF;
using OpenNos.Data;

namespace OpenNos.Mapper.Mappers
{
    public static class ShopMapper
    {
        #region Methods

        public static bool ToShop(ShopDTO input, Shop output)
        {
            if (input == null)
            {
                return false;
            }

            output.MapNpcId = input.MapNpcId;
            output.MenuType = input.MenuType;
            output.Name = input.NameI18NKey;
            output.ShopId = input.ShopId;
            output.ShopType = input.ShopType;

            return true;
        }

        public static bool ToShopDTO(Shop input, ShopDTO output)
        {
            if (input == null)
            {
                return false;
            }

            output.MapNpcId = input.MapNpcId;
            output.MenuType = input.MenuType;
            output.NameI18NKey = input.Name;
            output.ShopId = input.ShopId;
            output.ShopType = input.ShopType;

            return true;
        }

        #endregion
    }
}