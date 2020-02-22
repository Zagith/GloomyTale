using GloomyTale.Data;
using System.Collections.Generic;

namespace GloomyTale.DAL.Interface
{
    public interface IBoxItemDAO : IMappingBaseDAO
    {
        #region Methods

        IEnumerable<BoxItemDTO> LoadAll();

        #endregion
    }
}
