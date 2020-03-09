/*
 * This file is part of the OpenNos Emulator Project. See AUTHORS file for Copyright information
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 */

using GloomyTale.Core;
using GloomyTale.DAL.EF;
using GloomyTale.DAL.EF.Helpers;
using GloomyTale.DAL.Interface;
using GloomyTale.Data;
using GloomyTale.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace GloomyTale.DAL.DAO
{
    public class CellonOptionDAO : SynchronizableBaseDAO<CellonOption, CellonOptionDTO>, ICellonOptionDAO
    {
        public CellonOptionDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public DeleteResult DeleteByEquipmentSerialId(Guid id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    List<CellonOption> deleteentities = context.CellonOption.Where(s => s.EquipmentSerialId == id).ToList();
                    if (deleteentities.Count != 0)
                    {
                        context.CellonOption.RemoveRange(deleteentities);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_ERROR"), id, e.Message), e);
                return DeleteResult.Error;
            }
        }

        public IEnumerable<CellonOptionDTO> GetOptionsByWearableInstanceId(Guid wearableInstanceId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (CellonOption cellonOptionobject in context.CellonOption.Where(i => i.EquipmentSerialId.Equals(wearableInstanceId)))
                {
                    yield return _mapper.Map<CellonOptionDTO>(cellonOptionobject);
                }
            }
        }

        public void InsertOrUpdateFromList(List<CellonOptionDTO> cellonOption, Guid equipmentSerialId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (CellonOptionDTO item in cellonOption)
                    {
                        item.EquipmentSerialId = equipmentSerialId;
                        CellonOption entity = context.CellonOption.FirstOrDefault(c => c.EquipmentSerialId == item.EquipmentSerialId);
                        Save(item);
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        #endregion
    }
}