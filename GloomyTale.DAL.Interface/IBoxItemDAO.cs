using GloomyTale.Data;
using System.Collections.Generic;

namespace GloomyTale.DAL.Interface
{
    public interface IBoxItemDAO
    {
        #region Methods

        List<BoxItemDTO> LoadAll();

        #endregion
    }
}
