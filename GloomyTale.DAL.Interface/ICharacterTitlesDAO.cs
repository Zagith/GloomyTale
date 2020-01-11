using GloomyTale.Data;
using GloomyTale.Data.Enums;
using System.Collections.Generic;

namespace GloomyTale.DAL.Interface
{
    public interface ICharacterTitlesDAO
    {
        SaveResult InsertOrUpdate(ref CharacterTitleDTO characterTitle);

        IEnumerable<CharacterTitleDTO> LoadAll();

        CharacterTitleDTO LoadById(long characterId);

        IEnumerable<CharacterTitleDTO> LoadByCharacterId(long characterId);

    }
}
