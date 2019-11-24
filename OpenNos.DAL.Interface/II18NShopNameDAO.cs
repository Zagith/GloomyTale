using OpenNos.Data.Enums;
using OpenNos.Data.I18N;
using System.Collections.Generic;

namespace OpenNos.DAL.Interface
{
    public interface II18NShopNameDAO
    {
        #region Methods

        IEnumerable<I18NShopNameDto> FindByName(string name);

        I18NShopNameDto Insert(I18NShopNameDto teleporter);

        void Insert(List<I18NShopNameDto> skills);

        SaveResult InsertOrUpdate(I18NShopNameDto teleporter);

        IEnumerable<I18NShopNameDto> LoadAll();

        I18NShopNameDto LoadById(short teleporterId);

        #endregion
    }
}
