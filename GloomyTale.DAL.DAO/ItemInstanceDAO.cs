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

using AutoMapper;
using GloomyTale.Core;
using GloomyTale.DAL.EF;
using GloomyTale.DAL.EF.Helpers;
using GloomyTale.DAL.Interface;
using GloomyTale.Data;
using GloomyTale.Data.Enums;
using GloomyTale.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GloomyTale.DAL.DAO
{
    public class ItemInstanceDAO : SynchronizableBaseDAO<ItemInstance, ItemInstanceDTO>, IItemInstanceDAO
    {
        private readonly IItemInstanceMappingTypes _mappingTypes;

        public ItemInstanceDAO(IMapper mapper /*,IItemInstanceMappingTypes mappingTypes*/) : base(mapper)
        {
            //_mappingTypes = mappingTypes;
        }

        #region Methods

        public DeleteResult DeleteFromSlotAndType(long characterId, short slot, InventoryType type)
        {
            try
            {
                ItemInstanceDTO dto = LoadBySlotAndType(characterId, slot, type);
                if (dto != null)
                {
                    return Delete(dto.Id);
                }

                return DeleteResult.Unknown;
            }
            catch (Exception e)
            {
                Logger.Log.Error($"characterId: {characterId} slot: {slot} type: {type}", e);
                return DeleteResult.Error;
            }
        }

        public DeleteResult DeleteGuidList(IEnumerable<Guid> guids)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                try
                {
                    foreach (Guid id in guids)
                    {
                        ItemInstance entity = context.ItemInstance.FirstOrDefault(i => i.Id == id);
                        if (entity != null && entity.Type != InventoryType.FamilyWareHouse)
                        {
                            context.ItemInstance.Remove(entity);
                        }
                    }
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Logger.Log.LogUserEventError("DELETEGUIDLIST_EXCEPTION", "Saving Process","Items were not deleted!", ex);
                    foreach (Guid id in guids)
                    {
                        try
                        {
                            Delete(id);
                        }
                        catch (Exception exc)
                        {
                            // TODO: Work on: statement conflicted with the REFERENCE constraint
                            //       "FK_dbo.BazaarItem_dbo.ItemInstance_ItemInstanceId". The
                            //       conflict occurred in database "opennos", table "dbo.BazaarItem",
                            //       column 'ItemInstanceId'.
                            Logger.Log.LogUserEventError("ONSAVEDELETION_EXCEPTION", "Saving Process", $"FALLBACK FUNCTION FAILED! Detailed Item Information: Item ID = {id}", exc);
                        }
                    }
                }
                return DeleteResult.Deleted;
            }
        }

        public interface IItemInstanceMappingTypes
        {
            List<(Type, Type)> Types { get; }
        }

        protected override ItemInstanceDTO InsertOrUpdate(OpenNosContext context, ItemInstanceDTO itemInstance)
        {
            try
            {
                ItemInstance entity = context.ItemInstance.FirstOrDefault(c => c.Id == itemInstance.Id);

                itemInstance = entity == null ? Insert(itemInstance, context) : Update(entity, itemInstance, context);
                context.SaveChanges();
                return itemInstance;
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<ItemInstanceDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (ItemInstance itemInstance in context.ItemInstance.Where(i => i.CharacterId.Equals(characterId)))
                {
                    yield return _mapper.Map<ItemInstanceDTO>(itemInstance);
                }
            }
        }

        public ItemInstanceDTO LoadBySlotAndType(long characterId, short slot, InventoryType type)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    byte equipmentType = (byte)slot;
                    ItemInstance entity = context.ItemInstance.FirstOrDefault(i => i.CharacterId == characterId && i.Slot == equipmentType && i.Type == type);
                    return _mapper.Map<ItemInstanceDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<ItemInstanceDTO> LoadByType(long characterId, InventoryType type)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (ItemInstance itemInstance in context.ItemInstance.Where(i => i.CharacterId == characterId && i.Type == type))
                {
                    yield return _mapper.Map<ItemInstanceDTO>(itemInstance);
                }
            }
        }

        public IList<Guid> LoadSlotAndTypeByCharacterId(long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return context.ItemInstance.Where(i => i.CharacterId.Equals(characterId)).Select(i => i.Id).ToList();
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }


        protected override ItemInstance MapEntity(ItemInstanceDTO dto)
        {
            try
            {
                var entity = _mapper.Map<ItemInstance>(dto);
                (Type key, Type value) = _mappingTypes.Types.FirstOrDefault(k => k.Item1 == dto.GetType());
                if (key != null)
                {
                    entity = _mapper.Map(dto, key, value) as ItemInstance;
                }

                return entity;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        #endregion
    }
}