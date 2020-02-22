using GloomyTale.Data.Enums;
using GloomyTale.Data.I18N;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloomyTale.DAL.Interface
{
    public interface II18NSkillDAO : IMappingBaseDAO
    {
        #region Methods

        I18NSkillDto Insert(I18NSkillDto teleporter);

        void Insert(List<I18NSkillDto> skills);

        SaveResult InsertOrUpdate(I18NSkillDto teleporter);

        IEnumerable<I18NSkillDto> LoadAll();

        I18NSkillDto LoadById(short teleporterId);

        #endregion
    }
}
