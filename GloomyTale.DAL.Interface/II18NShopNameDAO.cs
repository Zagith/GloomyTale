using GloomyTale.Data.Enums;
using GloomyTale.Data.I18N;
using System.Collections.Generic;

namespace GloomyTale.DAL.Interface
{
    public interface II18NShopNameDAO : IMappingBaseDAO
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
