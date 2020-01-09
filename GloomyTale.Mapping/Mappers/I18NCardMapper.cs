using OpenNos.DAL.EF.Entities;
using OpenNos.Data.I18N;

namespace OpenNos.Mapper.Mappers
{
    public static class I18NCardMapper
    {
        #region Methods

        public static bool ToI18NCard(II18NCardDto input, I18NCard output)
        {
            if (input == null)
            {
                return false;
            }

            output.Key = input.Key;
            output.RegionType = input.RegionType;
            output.Text = input.Text;

            return true;
        }

        public static bool ToI18NCardDTO(I18NCard input, II18NCardDto output)
        {
            if (input == null)
            {
                return false;
            }

            output.Key = input.Key;
            output.RegionType = input.RegionType;
            output.Text = input.Text;

            return true;
        }

        #endregion
    }
}
