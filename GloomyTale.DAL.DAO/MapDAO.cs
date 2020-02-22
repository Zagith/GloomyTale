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
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace GloomyTale.DAL.DAO
{
    public class MapDAO : MappingBaseDao<Map, MapDTO>, IMapDAO
    {
        public MapDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public void Insert(List<MapDTO> maps)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (MapDTO Item in maps)
                    {
                        var entity = _mapper.Map<Map>(Item);
                        context.Map.Add(entity);
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        public MapDTO Insert(MapDTO map)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    if (context.Map.FirstOrDefault(c => c.MapId.Equals(map.MapId)) == null)
                    {
                        var entity = _mapper.Map<Map>(map);
                        context.Map.Add(entity);
                        context.SaveChanges();
                        return _mapper.Map<MapDTO>(entity);
                    }

                    return new MapDTO();
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<MapDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Map Map in context.Map)
                {
                    yield return _mapper.Map<MapDTO>(Map);
                }
            }
        }

        public MapDTO LoadById(short mapId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<MapDTO>(context.Map.FirstOrDefault(c => c.MapId.Equals(mapId)));
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