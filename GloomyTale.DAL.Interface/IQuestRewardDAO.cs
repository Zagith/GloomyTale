using GloomyTale.Data;
using System.Collections.Generic;

namespace GloomyTale.DAL.Interface
{
    public interface IQuestRewardDAO : IMappingBaseDAO
    {
        #region Methods

        QuestRewardDTO Insert(QuestRewardDTO questReward);

        void Insert(List<QuestRewardDTO> questRewards);

        List<QuestRewardDTO> LoadAll();

        IEnumerable<QuestRewardDTO> LoadByQuestId(long questId);

        #endregion
    }
}
