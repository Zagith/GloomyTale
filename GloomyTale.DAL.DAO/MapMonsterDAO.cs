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
    public class MapMonsterDAO : MappingBaseDao<MapMonster, MapMonsterDTO>, IMapMonsterDAO
    {
        public MapMonsterDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public IEnumerable<MapMonsterDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.MapMonster.ToArray().Select(_mapper.Map<MapMonsterDTO>);
            }
        }

        public DeleteResult DeleteById(int mapMonsterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    MapMonster monster = context.MapMonster.First(i => i.MapMonsterId.Equals(mapMonsterId));

                    if (monster != null)
                    {
                        context.MapMonster.Remove(monster);
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

        public bool DoesMonsterExist(int mapMonsterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.MapMonster.Any(i => i.MapMonsterId.Equals(mapMonsterId));
            }
        }

        public void Insert(IEnumerable<MapMonsterDTO> mapMonsters)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {

                    foreach (MapMonsterDTO monster in mapMonsters)
                    {
                        var entity = _mapper.Map<MapMonster>(monster);
                        context.MapMonster.Add(entity);
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        public MapMonsterDTO Insert(MapMonsterDTO mapMonster)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<MapMonster>(mapMonster);
                    context.MapMonster.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<MapMonsterDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public SaveResult Update(ref MapMonsterDTO mapMonster)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    int mapMonsterId = mapMonster.MapMonsterId;
                    MapMonster entity = context.MapMonster.FirstOrDefault(c => c.MapMonsterId.Equals(mapMonsterId));

                    mapMonster = update(entity, mapMonster, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_MAPMONSTER_ERROR"), mapMonster.MapMonsterId, e.Message), e);
                return SaveResult.Error;
            }
        }

        private MapMonsterDTO update(MapMonster entity, MapMonsterDTO mapMonster, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(mapMonster, entity);
                context.SaveChanges();
            }

            return _mapper.Map<MapMonsterDTO>(entity);

            return null;
        }

        public MapMonsterDTO LoadById(int mapMonsterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<MapMonsterDTO>(context.MapMonster.FirstOrDefault(i => i.MapMonsterId.Equals(mapMonsterId)));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<MapMonsterDTO> LoadFromMap(short mapId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.MapMonster.Where(c => c.MapId.Equals(mapId)).ToArray().Select(_mapper.Map<MapMonsterDTO>);
            }
        }

        #endregion
    }
}