﻿using GloomyTale.Data.Enums;
using GloomyTale.Data.I18N;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloomyTale.DAL.Interface
{
    public interface II18NNpcMonsterDAO : IMappingBaseDAO
    {
        #region Methods

        IEnumerable<II18NNpcMonsterDto> FindByName(string name);

        II18NNpcMonsterDto Insert(II18NNpcMonsterDto teleporter);

        void Insert(List<II18NNpcMonsterDto> skills);

        SaveResult InsertOrUpdate(II18NNpcMonsterDto teleporter);

        IEnumerable<II18NNpcMonsterDto> LoadAll();

        II18NNpcMonsterDto LoadById(short teleporterId);

        #endregion
    }
}
