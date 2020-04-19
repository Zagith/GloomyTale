using OpenNos.Data;
using OpenNos.Data.Enums;
using System;
using System.Collections.Generic;

namespace OpenNos.DAL.Interface
{
    public interface IRuneEffectDAO
    {
        #region Methods

        DeleteResult DeleteByEquipmentSerialId(Guid id);

        RuneEffectDTO InsertOrUpdate(RuneEffectDTO runeEffect);

        void InsertOrUpdateFromList(List<RuneEffectDTO> runeEffect, Guid equipmentSerialId);

        IEnumerable<RuneEffectDTO> LoadByEquipmentSerialId(Guid id);

        #endregion
    }
}