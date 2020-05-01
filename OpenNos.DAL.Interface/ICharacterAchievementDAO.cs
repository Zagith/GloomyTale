using OpenNos.Data.Achievements;
using System;
using System.Collections.Generic;

namespace OpenNos.DAL.Interface
{
    public interface ICharacterAchievementDAO
    {
        CharacterAchievementDTO InsertOrUpdate(CharacterAchievementDTO quest);

        IEnumerable<Guid> LoadKeysByCharacterId(long characterId);

        IEnumerable<CharacterAchievementDTO> LoadByCharacterId(long characterId);
    }
}
