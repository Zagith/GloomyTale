using GloomyTale.Data.Enums;
using GloomyTale.Data.I18N;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloomyTale.DAL.Interface
{
    public interface II18NCardDAO : IMappingBaseDAO
    {
        #region Methods

        I18NCardDto Insert(I18NCardDto teleporter);

        void Insert(List<I18NCardDto> skills);

        SaveResult InsertOrUpdate(I18NCardDto teleporter);

        IEnumerable<I18NCardDto> LoadAll();

        I18NCardDto LoadById(short teleporterId);

        #endregion
    }
}
