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
    public class PortalDAO : MappingBaseDao<Portal, PortalDTO>, IPortalDAO
    {
        public PortalDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public void Insert(List<PortalDTO> portals)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (PortalDTO Item in portals)
                    {
                        var entity = _mapper.Map<Portal>(Item);
                        context.Portal.Add(entity);
                    }
                    
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        public PortalDTO Insert(PortalDTO portal)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<Portal>(portal);
                    context.Portal.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<PortalDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<PortalDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.Portal.ToArray().Select(_mapper.Map<PortalDTO>);
            }
        }

        public IEnumerable<PortalDTO> LoadByMap(short mapId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Portal portal in context.Portal.Where(c => c.SourceMapId.Equals(mapId)))
                {
                    yield return _mapper.Map<PortalDTO>(portal);
                }
            }
        }

        #endregion
    }
}