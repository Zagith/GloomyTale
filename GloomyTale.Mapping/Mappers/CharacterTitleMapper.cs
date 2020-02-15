using GloomyTale.DAL.EF;
using GloomyTale.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloomyTale.Mapper.Mappers
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
