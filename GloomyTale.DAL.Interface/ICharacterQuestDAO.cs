using GloomyTale.Data;
using System.Collections.Generic;
using GloomyTale.Data.Enums;
using System;

namespace GloomyTale.DAL.Interface
{
    public interface ICharacterQuestDAO
    {
        #region Methods

        DeleteResult Delete(long characterId, long questId);

        CharacterQuestDTO InsertOrUpdate(CharacterQuestDTO quest);

        IEnumerable<CharacterQuestDTO> LoadByCharacterId(long characterId);

        IEnumerable<Guid> LoadKeysByCharacterId(long characterId);

        #endregion
    }
}