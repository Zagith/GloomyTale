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
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace GloomyTale.DAL.DAO
{
    public class ItemDAO : MappingBaseDao<Item, ItemDTO>, IItemDAO
    {
        public ItemDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public IEnumerable<ItemDTO> FindByName(string name)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Item item in context.Item.Where(s => string.IsNullOrEmpty(name) ? s.Name.Equals("") : s.Name.Contains(name)))
                {
                    yield return _mapper.Map<ItemDTO>(item);
                }
            }
        }

        public void Insert(IEnumerable<ItemDTO> items)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (ItemDTO Item in items)
                    {
                        var entity = _mapper.Map<Item>(Item);
                        context.Item.Add(entity);
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        public ItemDTO Insert(ItemDTO item)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<Item>(item);
                    context.Item.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<ItemDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<ItemDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Item item in context.Item)
                {
                    yield return _mapper.Map<ItemDTO>(item);
                }
            }
        }

        public ItemDTO LoadByKey(string vNum)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<ItemDTO>(context.Item.FirstOrDefault(i => i.Name.Equals(vNum)));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public ItemDTO LoadById(short ItemVnum)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<ItemDTO>(context.Item.FirstOrDefault(i => i.VNum.Equals(ItemVnum)));
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