using OpenNos.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.DAL.EF
{
    public class RuneEffect
    {
        #region Properties

        public Guid EquipmentSerialId { get; set; }

        [Key]
        public long RuneEffectId { get; set; }

        public byte EffectType { get; set; }

        public byte Effect { get; set; }

        public short Value { get; set; }

        public short CardId { get; set; }

        public byte EffectUpgrade { get; set; }

        #endregion
    }
}