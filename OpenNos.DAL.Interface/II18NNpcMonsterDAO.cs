using OpenNos.Data.Enums;
using OpenNos.Data.I18N;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.Interface
{
    public interface II18NNpcMonsterDAO
    {
        #region Methods

        II18NNpcMonsterDto Insert(II18NNpcMonsterDto teleporter);

        void Insert(List<II18NNpcMonsterDto> skills);

        SaveResult InsertOrUpdate(II18NNpcMonsterDto teleporter);

        IEnumerable<II18NNpcMonsterDto> LoadAll();

        II18NNpcMonsterDto LoadById(short teleporterId);

        #endregion
    }
}
