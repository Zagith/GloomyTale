using OpenNos.Data.Enums;
using OpenNos.Data.I18N;
using OpenNos.Domain.I18N;
using System.Collections.Generic;

namespace OpenNos.DAL.Interface
{
    public interface II18NNpcMonsterDAO
    {
        #region Methods

        IEnumerable<II18NNpcMonsterDto> FindByName(string name, RegionType regionType);

        II18NNpcMonsterDto Insert(II18NNpcMonsterDto teleporter);

        void Insert(List<II18NNpcMonsterDto> skills);

        SaveResult InsertOrUpdate(II18NNpcMonsterDto teleporter);

        IEnumerable<II18NNpcMonsterDto> LoadAll();

        II18NNpcMonsterDto LoadById(short teleporterId);

        #endregion
    }
}
