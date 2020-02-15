using OpenNos.Data.Enums;
using OpenNos.Data.I18N;
using System.Collections.Generic;

namespace OpenNos.DAL.Interface
{
    public interface II18NSkillDAO
    {
        #region Methods

        II18NSkillDto Insert(II18NSkillDto teleporter);

        void Insert(List<II18NSkillDto> skills);

        SaveResult InsertOrUpdate(II18NSkillDto teleporter);

        IEnumerable<II18NSkillDto> LoadAll();

        II18NSkillDto LoadById(short teleporterId);

        #endregion
    }
}
