using GloomyTale.DAL.EF.Entities;
using OpenNos.Data.I18N;

namespace OpenNos.Mapper.Mappers
{
    public static class I18NNpcMonsterMapper
    {
        #region Methods

        public static bool ToI18NNpcMonster(II18NNpcMonsterDto input, I18NNpcMonster output)
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

        public static bool ToI18NNpcMonsterDTO(I18NNpcMonster input, II18NNpcMonsterDto output)
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
