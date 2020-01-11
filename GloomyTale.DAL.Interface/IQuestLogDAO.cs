using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GloomyTale.Data;
using GloomyTale.Data.Enums;

namespace GloomyTale.DAL.Interface
{
    public interface IQuestLogDAO
    {
        SaveResult InsertOrUpdate(ref QuestLogDTO questLog);

        QuestLogDTO LoadById(long id);

        IEnumerable<QuestLogDTO> LoadByCharacterId(long id);
    }
}