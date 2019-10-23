using OpenNos.Data;
using OpenNos.Data.Enums;
using System.Collections.Generic;

namespace OpenNos.DAL.Interface
{
    public interface ICharacterTitlesDAO
    {
        SaveResult InsertOrUpdate(ref CharacterTitleDTO characterTitle);

        IEnumerable<CharacterTitleDTO> LoadAll();

        CharacterTitleDTO LoadById(long characterId);
    }
}
