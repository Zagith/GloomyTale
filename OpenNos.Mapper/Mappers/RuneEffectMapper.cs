using OpenNos.DAL.EF;
using OpenNos.Data;

namespace OpenNos.Mapper.Mappers
{
    public static class RuneEffectMapper
    {
        #region Methods

        public static bool ToRuneEffect(RuneEffectDTO input, RuneEffect output)
        {
            if (input == null)
            {
                return false;
            }
            output.EquipmentSerialId = input.EquipmentSerialId;
            output.RuneEffectId = input.RuneEffectId;
            output.EffectType = input.EffectType;
            output.Effect = input.Effect;
            output.Value = input.Value;
            output.CardId = input.CardId;
            output.EffectUpgrade = input.EffectUpgrade;
            return true;
        }

        public static bool ToRuneEffectDTO(RuneEffect input, RuneEffectDTO output)
        {
            if (input == null)
            {
                return false;
            }
            output.EquipmentSerialId = input.EquipmentSerialId;
            output.RuneEffectId = input.RuneEffectId;
            output.EffectType = input.EffectType;
            output.Effect = input.Effect;
            output.Value = input.Value;
            output.CardId = input.CardId;
            output.EffectUpgrade = input.EffectUpgrade;
            return true;
        }

        #endregion
    }
}