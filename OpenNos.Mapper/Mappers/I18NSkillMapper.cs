using OpenNos.DAL.EF.Entities;
using OpenNos.Data.I18N;

namespace OpenNos.Mapper.Mappers
{
    public static class I18NSkillMapper
    {
        #region Methods

        public static bool ToI18NSkill(II18NSkillDto input, I18NSkill output)
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

        public static bool ToI18NSkillDTO(I18NSkill input, II18NSkillDto output)
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
