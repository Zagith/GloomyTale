﻿/*
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
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper;

namespace GloomyTale.DAL.DAO
{
    public class BazaarItemDAO : MappingBaseDao<BazaarItem, BazaarItemDTO>, IBazaarItemDAO
    {
        public BazaarItemDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public DeleteResult Delete(long bazaarItemId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    BazaarItem BazaarItem = context.BazaarItem.FirstOrDefault(c => c.BazaarItemId.Equals(bazaarItemId));

                    if (BazaarItem != null)
                    {
                        context.BazaarItem.Remove(BazaarItem);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_ERROR"), bazaarItemId, e.Message), e);
                return DeleteResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(ref BazaarItemDTO bazaarItem)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long bazaarItemId = bazaarItem.BazaarItemId;
                    BazaarItem entity = context.BazaarItem.FirstOrDefault(c => c.BazaarItemId.Equals(bazaarItemId));

                    if (entity == null)
                    {
                        bazaarItem = insert(bazaarItem, context);
                        return SaveResult.Inserted;
                    }

                    bazaarItem = update(entity, bazaarItem, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error($"BazaarItemId: {bazaarItem.BazaarItemId} Message: {e.Message}", e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<BazaarItemDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (BazaarItem bazaarItem in context.BazaarItem)
                {
                    yield return _mapper.Map<BazaarItemDTO>(bazaarItem);
                }
            }
        }

        public BazaarItemDTO LoadById(long bazaarItemId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<BazaarItemDTO>(context.BazaarItem.FirstOrDefault(i => i.BazaarItemId.Equals(bazaarItemId)));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public void RemoveOutDated()
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (BazaarItem entity in context.BazaarItem.Where(e => e.DateStart.AddDays(e.MedalUsed ? 30 : 7).AddHours(e.Duration) < DateTime.Now))
                    {
                        context.BazaarItem.Remove(entity);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        private BazaarItemDTO insert(BazaarItemDTO bazaarItem, OpenNosContext context)
        {
            var entity = _mapper.Map<BazaarItem>(bazaarItem);
            context.BazaarItem.Add(entity);
            context.SaveChanges();
            return _mapper.Map<BazaarItemDTO>(entity);
        }

        private BazaarItemDTO update(BazaarItem entity, BazaarItemDTO bazaarItem, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(bazaarItem, entity);
                context.SaveChanges();
            }

            return _mapper.Map<BazaarItemDTO>(entity);
        }

        #endregion
    }
}