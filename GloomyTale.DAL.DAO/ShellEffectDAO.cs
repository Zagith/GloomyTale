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
    public class ShellEffectDAO : MappingBaseDao<ShellEffect, ShellEffectDTO>, IShellEffectDAO
    {
        public ShellEffectDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public DeleteResult DeleteByEquipmentSerialId(Guid id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    List<ShellEffect> deleteentities = context.ShellEffect.Where(s => s.EquipmentSerialId == id).ToList();
                    if (deleteentities.Count != 0)
                    {
                        context.ShellEffect.RemoveRange(deleteentities);
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

        public ShellEffectDTO InsertOrUpdate(ShellEffectDTO shelleffect)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long shelleffectId = shelleffect.ShellEffectId;
                    ShellEffect entity = context.ShellEffect.FirstOrDefault(c => c.ShellEffectId.Equals(shelleffectId));

                    if (entity == null)
                    {
                        return insert(shelleffect, context);
                    }
                    return update(entity, shelleffect, context);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("INSERT_ERROR"), shelleffect, e.Message), e);
                return shelleffect;
            }
        }

        public void InsertOrUpdateFromList(List<ShellEffectDTO> shellEffects, Guid equipmentSerialId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    void insert(ShellEffectDTO shelleffect)
                    {
                        var entity = _mapper.Map<ShellEffect>(shelleffect);
                        context.ShellEffect.Add(entity);
                        context.SaveChanges();
                        shelleffect.ShellEffectId = entity.ShellEffectId;
                    }

                    void update(ShellEffect _entity, ShellEffectDTO shelleffect)
                    {
                        if (_entity != null)
                        {
                            _mapper.Map(shelleffect, _entity);
                        }
                    }

                    foreach (ShellEffectDTO item in shellEffects)
                    {
                        item.EquipmentSerialId = equipmentSerialId;
                        ShellEffect entity = context.ShellEffect.FirstOrDefault(c => c.ShellEffectId == item.ShellEffectId);

                        if (entity == null)
                        {
                            insert(item);
                        }
                        else
                        {
                            update(entity, item);
                        }
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        public IEnumerable<ShellEffectDTO> LoadByEquipmentSerialId(Guid id)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (ShellEffect cellonOptionobject in context.ShellEffect.Where(i => i.EquipmentSerialId.Equals(id)))
                {
                    yield return _mapper.Map<ShellEffectDTO>(cellonOptionobject);
                }
            }
        }

        private ShellEffectDTO insert(ShellEffectDTO shelleffect, OpenNosContext context)
        {
            var entity = _mapper.Map<ShellEffect>(shelleffect);
            context.ShellEffect.Add(entity);
            context.SaveChanges();
            return _mapper.Map<ShellEffectDTO>(entity);
        }

        private ShellEffectDTO update(ShellEffect entity, ShellEffectDTO shelleffect, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(shelleffect, entity);
                context.SaveChanges();
            }

            return _mapper.Map<ShellEffectDTO>(entity);
        }

        #endregion
    }
}