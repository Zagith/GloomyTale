using Mapster;
using GloomyTale.DAL.EF;
using OpenNos.Data;
using OpenNos.Data.Base;

namespace OpenNos.Mapper.Mappers
{
    public static class CardMapper
    {
        #region Methods

        public static bool ToCard(CardDTO input, Card output)
        {
            if (input == null)
            {
                return false;
            }
            TypeAdapterConfig.GlobalSettings.Default.IgnoreAttribute(typeof(I18NFromAttribute));
            output.BuffType = input.BuffType;
            output.CardId = input.CardId;
            output.Delay = input.Delay;
            output.Duration = input.Duration;
            output.EffectId = input.EffectId;
            output.Level = input.Level;
            output.Name = input.NameI18NKey;
            output.Propability = input.Propability;
            output.TimeoutBuff = input.TimeoutBuff;
            output.TimeoutBuffChance = input.TimeoutBuffChance;

            return true;
        }

        public static bool ToCardDTO(Card input, CardDTO output)
        {
            if (input == null)
            {
                return false;
            }
            TypeAdapterConfig.GlobalSettings.Default.IgnoreAttribute(typeof(I18NFromAttribute));
            output.BuffType = input.BuffType;
            output.CardId = input.CardId;
            output.Delay = input.Delay;
            output.Duration = input.Duration;
            output.EffectId = input.EffectId;
            output.Level = input.Level;
            output.NameI18NKey = input.Name;
            output.Propability = input.Propability;
            output.TimeoutBuff = input.TimeoutBuff;
            output.TimeoutBuffChance = input.TimeoutBuffChance;

            return true;
        }

        #endregion
    }
}