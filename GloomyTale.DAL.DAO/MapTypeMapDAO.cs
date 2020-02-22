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
    public class MapTypeMapDAO : MappingBaseDao<MapTypeMap, MapTypeMapDTO>, IMapTypeMapDAO
    {
        public MapTypeMapDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public void Insert(List<MapTypeMapDTO> mapTypeMaps)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {

                    foreach (MapTypeMapDTO mapTypeMap in mapTypeMaps)
                    {
                        var entity = _mapper.Map<MapTypeMap>(mapTypeMap);
                        context.MapTypeMap.Add(entity);
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        public IEnumerable<MapTypeMapDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (MapTypeMap MapTypeMap in context.MapTypeMap)
                {
                    yield return _mapper.Map<MapTypeMapDTO>(MapTypeMap);
                }
            }
        }

        public MapTypeMapDTO LoadByMapAndMapType(short mapId, short maptypeId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<MapTypeMapDTO>(context.MapTypeMap.FirstOrDefault(i => i.MapId.Equals(mapId) && i.MapTypeId.Equals(maptypeId)));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<MapTypeMapDTO> LoadByMapId(short mapId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (MapTypeMap MapTypeMap in context.MapTypeMap.Where(c => c.MapId.Equals(mapId)))
                {
                    yield return _mapper.Map<MapTypeMapDTO>(MapTypeMap);
                }
            }
        }

        public IEnumerable<MapTypeMapDTO> LoadByMapTypeId(short maptypeId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (MapTypeMap MapTypeMap in context.MapTypeMap.Where(c => c.MapTypeId.Equals(maptypeId)))
                {
                    yield return _mapper.Map<MapTypeMapDTO>(MapTypeMap);
                }
            }
        }

        #endregion
    }
}