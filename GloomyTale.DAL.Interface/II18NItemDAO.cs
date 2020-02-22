﻿using GloomyTale.Data.Enums;
using GloomyTale.Data.I18N;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloomyTale.DAL.Interface
{
    public interface II18NItemDAO : IMappingBaseDAO
    {
        #region Methods

        IEnumerable<I18NItemDto> FindByName(string name);

        I18NItemDto Insert(I18NItemDto teleporter);

        void Insert(List<I18NItemDto> skills);

        SaveResult InsertOrUpdate(I18NItemDto teleporter);

        IEnumerable<I18NItemDto> LoadAll();

        I18NItemDto LoadById(short teleporterId);

        #endregion
    }
}