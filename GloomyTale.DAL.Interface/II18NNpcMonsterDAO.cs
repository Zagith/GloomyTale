using GloomyTale.Data.Enums;
using GloomyTale.Data.I18N;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloomyTale.DAL.Interface
{
    public interface II18NNpcMonsterDAO : IMappingBaseDAO
    {
        #region Methods

        IEnumerable<I18NNpcMonsterDto> FindByName(string name);

        I18NNpcMonsterDto Insert(I18NNpcMonsterDto teleporter);

        void Insert(List<I18NNpcMonsterDto> skills);

        SaveResult InsertOrUpdate(I18NNpcMonsterDto teleporter);

        IEnumerable<I18NNpcMonsterDto> LoadAll();

        I18NNpcMonsterDto LoadById(short teleporterId);

        #endregion
    }
}
