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
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper;

namespace GloomyTale.DAL.DAO
{
    public class MinigameLogDAO : MappingBaseDao<MinigameLog, MinigameLogDTO>, IMinigameLogDAO
    {
        public MinigameLogDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public SaveResult InsertOrUpdate(ref MinigameLogDTO minigameLog)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long minigameLogId = minigameLog.MinigameLogId;
                    MinigameLog entity = context.MinigameLog.FirstOrDefault(c => c.MinigameLogId.Equals(minigameLogId));

                    if (entity == null)
                    {
                        minigameLog = insert(minigameLog, context);
                        return SaveResult.Inserted;
                    }
                    minigameLog = update(entity, minigameLog, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<MinigameLogDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (MinigameLog obj in context.MinigameLog.Where(s => s.CharacterId == characterId))
                {
                    yield return _mapper.Map<MinigameLogDTO>(obj);
                }
            }
        }

        public MinigameLogDTO LoadById(long minigameLogId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<MinigameLogDTO>(context.MinigameLog.FirstOrDefault(s => s.MinigameLogId.Equals(minigameLogId)));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
            return null;
        }

        private MinigameLogDTO insert(MinigameLogDTO account, OpenNosContext context)
        {
            var entity = _mapper.Map<MinigameLog>(account);
            context.MinigameLog.Add(entity);
            context.SaveChanges();
            return _mapper.Map<MinigameLogDTO>(entity);
        }

        private MinigameLogDTO update(MinigameLog entity, MinigameLogDTO account, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(account, entity);
                context.SaveChanges();
            }

            return _mapper.Map<MinigameLogDTO>(entity);
        }

        #endregion
    }
}