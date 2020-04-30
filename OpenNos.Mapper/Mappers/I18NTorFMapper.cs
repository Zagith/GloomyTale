using OpenNos.DAL.EF.Entities;
using OpenNos.Data.I18N;

namespace OpenNos.Mapper.Mappers
{
    public static class I18NTorFMapper
    {
        #region Methods

        public static bool ToI18NTorF(I18NTorFDto input, I18NTorF output)
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

        public static bool ToI18NTorFDTO(I18NTorF input, I18NTorFDto output)
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
