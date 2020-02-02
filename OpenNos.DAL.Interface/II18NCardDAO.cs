using OpenNos.Data.Enums;
using OpenNos.Data.I18N;
using System.Collections.Generic;

namespace OpenNos.DAL.Interface
{
    public interface II18NCardDAO
    {
        #region Methods

        II18NCardDto Insert(II18NCardDto teleporter);

        void Insert(List<II18NCardDto> skills);

        SaveResult InsertOrUpdate(II18NCardDto teleporter);

        IEnumerable<II18NCardDto> LoadAll();

        II18NCardDto LoadById(short teleporterId);

        #endregion
    }
}
