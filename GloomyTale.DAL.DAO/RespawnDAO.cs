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
    public class RespawnDAO : MappingBaseDao<Respawn, RespawnDTO>, IRespawnDAO
    {
        public RespawnDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public SaveResult InsertOrUpdate(ref RespawnDTO respawn)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long CharacterId = respawn.CharacterId;
                    long RespawnMapTypeId = respawn.RespawnMapTypeId;
                    Respawn entity = context.Respawn.FirstOrDefault(c => c.RespawnMapTypeId.Equals(RespawnMapTypeId) && c.CharacterId.Equals(CharacterId));

                    if (entity == null)
                    {
                        respawn = insert(respawn, context);
                        return SaveResult.Inserted;
                    }

                    respawn.RespawnId = entity.RespawnId;
                    respawn = update(entity, respawn, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<RespawnDTO> LoadByCharacter(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Respawn Respawnobject in context.Respawn.Where(i => i.CharacterId.Equals(characterId)))
                {
                    yield return _mapper.Map<RespawnDTO>(Respawnobject);
                }
            }
        }

        public RespawnDTO LoadById(long respawnId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<RespawnDTO>(context.Respawn.FirstOrDefault(s => s.RespawnId.Equals(respawnId)));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        private RespawnDTO insert(RespawnDTO respawn, OpenNosContext context)
        {
            try
            {
                var entity = _mapper.Map<Respawn>(respawn);
                context.Respawn.Add(entity);
                context.SaveChanges();
                return _mapper.Map<RespawnDTO>(entity);
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        private RespawnDTO update(Respawn entity, RespawnDTO respawn, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(respawn, entity);
                context.SaveChanges();
            }

            return _mapper.Map<RespawnDTO>(entity);
        }

        #endregion
    }
}