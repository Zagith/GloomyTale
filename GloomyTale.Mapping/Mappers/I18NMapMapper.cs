using OpenNos.DAL.EF.Entities;
using OpenNos.Data.I18N;

namespace OpenNos.Mapper.Mappers
{
    public static class I18NMapMapper
    {
        #region Methods

        public static bool ToI18NMap(II18NMapDto input, I18NMapPointData output)
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

        public static bool ToI18NMapDTO(I18NMapPointData input, II18NMapDto output)
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
