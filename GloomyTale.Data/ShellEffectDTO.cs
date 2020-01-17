using GloomyTale.Domain;
using System;

namespace GloomyTale.Data
{
    [Serializable]
    public class ShellEffectDTO : MappingBaseDTO
    {
        #region Properties

        public byte Effect { get; set; }

        public ShellEffectLevelType EffectLevel { get; set; }

        public Guid EquipmentSerialId { get; set; }

        public long ShellEffectId { get; set; }

        public short Value { get; set; }

        #endregion
    }
}