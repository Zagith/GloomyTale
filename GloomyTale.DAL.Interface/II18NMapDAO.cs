using GloomyTale.Data.Enums;
using GloomyTale.Data.I18N;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloomyTale.DAL.Interface
{
    public interface II18NMapDAO : IMappingBaseDAO
    {
        #region Methods

        I18NMapPointDataDto Insert(I18NMapPointDataDto teleporter);

        void Insert(List<I18NMapPointDataDto> skills);

        SaveResult InsertOrUpdate(I18NMapPointDataDto teleporter);

        IEnumerable<I18NMapPointDataDto> LoadAll();

        I18NMapPointDataDto LoadById(short teleporterId);

        #endregion
    }
}
