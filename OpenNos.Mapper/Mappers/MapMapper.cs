using OpenNos.DAL.EF;
using OpenNos.Data;

namespace OpenNos.Mapper.Mappers
{
    public static class MapMapper
    {
        #region Methods

        public static bool ToMap(MapDTO input, Map output)
        {
            if (input == null)
            {
                return false;
            }

            output.Data = input.Data;
            output.MapId = input.MapId;
            output.GridMapId = input.GridMapId;
            output.Music = input.Music;
            output.Name = input.NameI18NKey;
            output.ShopAllowed = input.ShopAllowed;
            output.XpRate = input.XpRate;
            output.MeteoriteLevel = input.MeteoriteLevel;

            return true;
        }

        public static bool ToMapDTO(Map input, MapDTO output)
        {
            if (input == null)
            {
                return false;
            }

            output.Data = input.Data;
            output.MapId = input.MapId;
            output.GridMapId = input.GridMapId;
            output.Music = input.Music;
            output.NameI18NKey = input.Name;
            output.ShopAllowed = input.ShopAllowed;
            output.XpRate = input.XpRate;
            output.MeteoriteLevel = input.MeteoriteLevel;

            return true;
        }

        #endregion
    }
}