﻿using GloomyTale.Data.Enums;
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

        II18NCardDto Insert(II18NCardDto teleporter);

        void Insert(List<II18NCardDto> skills);

        SaveResult InsertOrUpdate(II18NCardDto teleporter);

        IEnumerable<II18NCardDto> LoadAll();

        II18NCardDto LoadById(short teleporterId);

        #endregion
    }
}