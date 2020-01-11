﻿using GloomyTale.Data;
using GloomyTale.Data.Enums;
using System.Collections.Generic;

namespace GloomyTale.DAL.Interface
{
    public interface IQuestDAO
    {
        #region Methods

        DeleteResult DeleteById(long id);

        QuestDTO InsertOrUpdate(QuestDTO quest);

        void Insert(List<QuestDTO> questList);

        QuestDTO LoadById(long id);

        IEnumerable<QuestDTO> LoadAll();

        #endregion
    }
}