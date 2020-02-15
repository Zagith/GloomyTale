using OpenNos.Data;
using OpenNos.Data.Enums;
using System.Collections.Generic;

namespace OpenNos.DAL.Interface
{
    public interface IQuestLogDAO
    {
        SaveResult InsertOrUpdate(ref QuestLogDTO questLog);

        QuestLogDTO LoadById(long id);

        IEnumerable<QuestLogDTO> LoadByCharacterId(long id);
    }
}