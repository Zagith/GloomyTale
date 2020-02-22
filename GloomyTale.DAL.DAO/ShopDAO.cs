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

using Microsoft.EntityFrameworkCore;
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
    public class ShopDAO : MappingBaseDao<Shop, ShopDTO>, IShopDAO
    {
        public ShopDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public DeleteResult DeleteByNpcId(int mapNpcId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Shop shop = context.Shop.First(i => i.MapNpcId.Equals(mapNpcId));
                    IEnumerable<ShopItem> shopItem = context.ShopItem.Where(s => s.ShopId.Equals(shop.ShopId));
                    IEnumerable<ShopSkill> shopSkill = context.ShopSkill.Where(s => s.ShopId.Equals(shop.ShopId));

                    if (shop != null)
                    {
                        foreach (ShopItem item in shopItem)
                        {
                            context.ShopItem.Remove(item);
                        }
                        foreach (ShopSkill skill in shopSkill)
                        {
                            context.ShopSkill.Remove(skill);
                        }
                        context.Shop.Remove(shop);
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

        public void Insert(List<ShopDTO> shops)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {

                    foreach (ShopDTO Item in shops)
                    {
                        var entity = _mapper.Map<Shop>(Item);
                        context.Shop.Add(entity);
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        public ShopDTO Insert(ShopDTO shop)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    if (context.Shop.FirstOrDefault(c => c.MapNpcId.Equals(shop.MapNpcId)) == null)
                    {
                        var entity = _mapper.Map<Shop>(shop);
                        context.Shop.Add(entity);
                        context.SaveChanges();
                        return _mapper.Map<ShopDTO>(entity);
                    }

                    return new ShopDTO();
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        private ShopDTO insert(ShopDTO shop, OpenNosContext context)
        {
            try
            {
                var entity = _mapper.Map<Shop>(shop);
                context.Shop.Add(entity);
                context.SaveChanges();
                return _mapper.Map<ShopDTO>(entity);
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        private ShopDTO update(Shop entity, ShopDTO shop, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(shop, entity);
                context.SaveChanges();
            }

            return _mapper.Map<ShopDTO>(entity);
        }

        public SaveResult Update(ref ShopDTO shop)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    int shopId = shop.ShopId;
                    Shop entity = context.Shop.FirstOrDefault(c => c.ShopId.Equals(shopId));
                    
                    shop = update(entity, shop, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_SHOP_ERROR"), shop.ShopId, e.Message), e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<ShopDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Shop entity in context.Shop.ToArray())
                {
                    yield return _mapper.Map<ShopDTO>(entity);
                }
            }
        }

        public ShopDTO LoadById(int shopId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<ShopDTO>(context.Shop.FirstOrDefault(s => s.ShopId.Equals(shopId)));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public ShopDTO LoadByNpc(int mapNpcId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<ShopDTO>(context.Shop.FirstOrDefault(s => s.MapNpcId.Equals(mapNpcId)));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        #endregion
    }
}