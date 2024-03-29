﻿using OpenNos.Data.Enums;
using OpenNos.Data.I18N;
using OpenNos.Domain.I18N;
using System.Collections.Generic;

namespace OpenNos.DAL.Interface
{
    public interface II18NItemDAO
    {
        #region Methods

        IEnumerable<I18NItemDto> FindByName(string name, RegionType regionType);

        I18NItemDto Insert(I18NItemDto teleporter);

        void Insert(List<I18NItemDto> skills);

        SaveResult InsertOrUpdate(I18NItemDto teleporter);

        IEnumerable<I18NItemDto> LoadAll();

        I18NItemDto LoadById(short teleporterId);

        #endregion
    }
}
