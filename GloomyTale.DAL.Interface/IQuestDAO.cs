using GloomyTale.Data;
using GloomyTale.Data.Enums;
using System.Collections.Generic;

namespace GloomyTale.DAL.Interface
{
    public interface IQuestDAO : IMappingBaseDAO
    {
        #region Methods

        DeleteResult DeleteById(long id);

        void Insert(List<QuestDTO> questList);

        QuestDTO LoadById(long id);

        IEnumerable<QuestDTO> LoadAll();

        #endregion
    }
}