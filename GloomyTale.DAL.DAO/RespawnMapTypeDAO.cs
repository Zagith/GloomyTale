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
    public class RespawnMapTypeDAO : MappingBaseDao<RespawnMapType, RespawnMapTypeDTO>, IRespawnMapTypeDAO
    {
        public RespawnMapTypeDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public void Insert(List<RespawnMapTypeDTO> respawnMapTypes)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {

                    foreach (RespawnMapTypeDTO RespawnMapType in respawnMapTypes)
                    {
                        var entity = _mapper.Map<RespawnMapType>(RespawnMapType);
                        context.RespawnMapType.Add(entity);
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        public SaveResult InsertOrUpdate(ref RespawnMapTypeDTO respawnMapType)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    short mapId = respawnMapType.DefaultMapId;
                    RespawnMapType entity = context.RespawnMapType.FirstOrDefault(c => c.DefaultMapId.Equals(mapId));

                    if (entity == null)
                    {
                        respawnMapType = insert(respawnMapType, context);
                        return SaveResult.Inserted;
                    }

                    respawnMapType.RespawnMapTypeId = entity.RespawnMapTypeId;
                    respawnMapType = update(entity, respawnMapType, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return SaveResult.Error;
            }
        }

        public RespawnMapTypeDTO LoadById(long respawnMapTypeId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<RespawnMapTypeDTO>(context.RespawnMapType.FirstOrDefault(s => s.RespawnMapTypeId.Equals(respawnMapTypeId)));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<RespawnMapTypeDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.RespawnMapType.ToArray().Select(_mapper.Map<RespawnMapTypeDTO>);
            }
        }

        public RespawnMapTypeDTO LoadByMapId(short mapId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<RespawnMapTypeDTO>(context.RespawnMapType.FirstOrDefault(s => s.DefaultMapId.Equals(mapId)));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        private RespawnMapTypeDTO insert(RespawnMapTypeDTO respawnMapType, OpenNosContext context)
        {
            try
            {
                var entity = _mapper.Map<RespawnMapType>(respawnMapType);
                context.RespawnMapType.Add(entity);
                context.SaveChanges();
                return _mapper.Map<RespawnMapTypeDTO>(entity);
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        private RespawnMapTypeDTO update(RespawnMapType entity, RespawnMapTypeDTO respawnMapType, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(respawnMapType, entity);
                context.SaveChanges();
            }

            return _mapper.Map<RespawnMapTypeDTO>(entity);
        }

        #endregion
    }
}