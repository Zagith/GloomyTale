using GloomyTale.Data;
using GloomyTale.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.DAL.Interface
{
    public interface ISynchronizableBaseDAO<TDTO> : IMappingBaseDAO
    where TDTO : SynchronizableBaseDTO
    {
        #region Methods

        DeleteResult Delete(Guid id);

        DeleteResult Delete(IEnumerable<Guid> ids);

        TDTO LoadById(Guid id);

        TDTO Save(TDTO obj);

        void Save(IEnumerable<TDTO> objs);
        #endregion
    }
}
