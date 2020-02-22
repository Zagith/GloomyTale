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
    public class ShopItemDAO : MappingBaseDao<ShopItem, ShopItemDTO>, IShopItemDAO
    {
        public ShopItemDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public DeleteResult DeleteById(int itemId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    ShopItem Item = context.ShopItem.FirstOrDefault(i => i.ShopItemId.Equals(itemId));

                    if (Item != null)
                    {
                        context.ShopItem.Remove(Item);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return DeleteResult.Error;
            }
        }

        public ShopItemDTO Insert(ShopItemDTO item)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<ShopItem>(item);
                    context.ShopItem.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<ShopItemDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public void Insert(List<ShopItemDTO> items)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (ShopItemDTO Item in items)
                    {
                        var entity = _mapper.Map<ShopItem>(Item);
                        context.ShopItem.Add(entity);
                    }


                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public IEnumerable<ShopItemDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (ShopItem entity in context.ShopItem)
                {
                    yield return _mapper.Map<ShopItemDTO>(entity);
                }
            }
        }

        public ShopItemDTO LoadById(int itemId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<ShopItemDTO>(context.ShopItem.FirstOrDefault(i => i.ShopItemId.Equals(itemId)));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<ShopItemDTO> LoadByShopId(int shopId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (ShopItem ShopItem in context.ShopItem.Where(i => i.ShopId.Equals(shopId)))
                {
                    yield return _mapper.Map<ShopItemDTO>(ShopItem);
                }
            }
        }

        #endregion
    }
}