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
    public class MapTypeDAO : MappingBaseDao<MapType, MapTypeDTO>, IMapTypeDAO
    {
        public MapTypeDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public MapTypeDTO Insert(ref MapTypeDTO mapType)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<MapType>(mapType);
                    context.MapType.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<MapTypeDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<MapTypeDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (MapType MapType in context.MapType)
                {
                    yield return _mapper.Map<MapTypeDTO>(MapType);
                }
            }
        }

        public MapTypeDTO LoadById(short maptypeId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<MapTypeDTO>(context.MapType.FirstOrDefault(s => s.MapTypeId.Equals(maptypeId)));
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