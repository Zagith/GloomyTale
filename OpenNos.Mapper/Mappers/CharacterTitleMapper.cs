using OpenNos.DAL.EF;
using OpenNos.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Mapper.Mappers
{
    public class CharacterTitleMapper
    {
        public static bool ToCharacterTitle(CharacterTitleDTO input, CharacterTitle output)
        {
            if (input == null)
            {
                return false;
            }

            output.CharacterTitleId = input.CharacterTitleId;
            output.CharacterId = input.CharacterId;
            output.Active = input.Active;
            output.Visible = input.Visible;
            output.TitleType = input.TitleType;

            return true;
        }

        public static bool ToCharacterTitleDTO(CharacterTitle input, CharacterTitleDTO output)
        {
            if (input == null)
            {
                return false;
            }

            output.CharacterTitleId = input.CharacterTitleId;
            output.CharacterId = input.CharacterId;
            output.Active = input.Active;
            output.Visible = input.Visible;
            output.TitleType = input.TitleType;

            return true;
        }
    }
}
