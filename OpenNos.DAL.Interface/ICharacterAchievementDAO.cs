using OpenNos.Data.Achievements;
using OpenNos.Data.Enums;
using System;
using System.Collections.Generic;

namespace OpenNos.DAL.Interface
{
    public interface ICharacterAchievementDAO
    {
        DeleteResult Delete(long characterId, long questId);

        CharacterAchievementDTO InsertOrUpdate(CharacterAchievementDTO quest);

        IEnumerable<Guid> LoadKeysByCharacterId(long characterId);

        IEnumerable<CharacterAchievementDTO> LoadByCharacterId(long characterId);
    }
}
