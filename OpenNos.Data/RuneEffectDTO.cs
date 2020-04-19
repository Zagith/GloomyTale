using System;

namespace OpenNos.Data
{
    [Serializable]
    public class RuneEffectDTO
    {
        #region Properties

        public Guid EquipmentSerialId { get; set; }

        public long RuneEffectId { get; set; }

        public byte EffectType { get; set; }

        public byte Effect { get; set; }

        public short Value { get; set; }

        public short CardId { get; set; }

        public byte EffectUpgrade { get; set; }

        #endregion
    }
}