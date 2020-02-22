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
    public class MapNpcDAO : MappingBaseDao<MapNpc, MapNpcDTO>, IMapNpcDAO
    {
        public MapNpcDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public DeleteResult DeleteById(int mapNpcId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    MapNpc npc = context.MapNpc.First(i => i.MapNpcId.Equals(mapNpcId));

                    if (npc != null)
                    {
                        context.MapNpc.Remove(npc);
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
        public bool DoesNpcExist(int mapNpcId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.MapNpc.Any(i => i.MapNpcId.Equals(mapNpcId));
            }
        }
        public void Insert(List<MapNpcDTO> npcs)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {

                    foreach (MapNpcDTO Item in npcs)
                    {
                        var entity = _mapper.Map<MapNpc>(Item);
                        context.MapNpc.Add(entity);
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        public MapNpcDTO Insert(MapNpcDTO npc)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<MapNpc>(npc);
                    context.MapNpc.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<MapNpcDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public SaveResult Update(ref MapNpcDTO mapNpc)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    int mapNpcId = mapNpc.MapNpcId;
                    MapNpc entity = context.MapNpc.FirstOrDefault(c => c.MapNpcId.Equals(mapNpcId));

                    mapNpc = update(entity, mapNpc, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_MAPNPC_ERROR"), mapNpc.MapNpcId, e.Message), e);
                return SaveResult.Error;
            }
        }

        private MapNpcDTO update(MapNpc entity, MapNpcDTO mapNpc, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(mapNpc, entity);
                context.SaveChanges();
            }

            return _mapper.Map<MapNpcDTO>(entity);
        }

        public IEnumerable<MapNpcDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.MapNpc.ToArray().Select(_mapper.Map<MapNpcDTO>);
            }
        }

        public MapNpcDTO LoadById(int mapNpcId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<MapNpcDTO>(context.MapNpc.FirstOrDefault(i => i.MapNpcId.Equals(mapNpcId)));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<MapNpcDTO> LoadFromMap(short mapId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.MapNpc.Where(s => s.MapId == mapId).ToArray().Select(_mapper.Map<MapNpcDTO>);
            }
        }

        #endregion
    }
}