﻿using OpenNos.Data.Enums;
using OpenNos.Data.I18N;
using System.Collections.Generic;

namespace OpenNos.DAL.Interface
{
    public interface II18NMapDAO
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
