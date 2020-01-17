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

        II18NMapDto Insert(II18NMapDto teleporter);

        void Insert(List<II18NMapDto> skills);

        SaveResult InsertOrUpdate(II18NMapDto teleporter);

        IEnumerable<II18NMapDto> LoadAll();

        II18NMapDto LoadById(short teleporterId);

        #endregion
    }
}
