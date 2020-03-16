using OpenNos.DAL.EF.Entities;
using OpenNos.Data;

namespace OpenNos.Mapper.Mappers
{
    public static class TrueOrFalseMapping
    {
        #region Methods

        public static bool ToTrueOrFalse(TrueOrFalseDTO input, TrueOrFalse output)
        {
            if (input == null)
            {
                return false;
            }

            output.Question = input.Question;
            output.Answer = input.Answer;
            output.QuestionType = input.QuestionType;

            return true;
        }

        public static bool ToTrueOrFalseDTO(TrueOrFalse input, TrueOrFalseDTO output)
        {
            if (input == null)
            {
                return false;
            }

            output.Question = input.Question;
            output.Answer = input.Answer;
            output.QuestionType = input.QuestionType;

            return true;
        }

        #endregion
    }
}
