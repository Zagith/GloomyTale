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
    public class ScriptedInstanceDAO : MappingBaseDao<ScriptedInstance, ScriptedInstanceDTO>, IScriptedInstanceDAO
    {
        public ScriptedInstanceDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public void Insert(List<ScriptedInstanceDTO> scriptedInstances)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (ScriptedInstanceDTO Item in scriptedInstances)
                    {
                        var entity = _mapper.Map<ScriptedInstance>(Item);
                        context.ScriptedInstance.Add(entity);
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        public ScriptedInstanceDTO Insert(ScriptedInstanceDTO scriptedInstance)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<ScriptedInstance>(scriptedInstance);
                    context.ScriptedInstance.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<ScriptedInstanceDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<ScriptedInstanceDTO> LoadByMap(short mapId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (ScriptedInstance timespaceObject in context.ScriptedInstance.Where(c => c.MapId.Equals(mapId)))
                {
                    yield return _mapper.Map<ScriptedInstanceDTO>(timespaceObject);
                }
            }
        }

        #endregion
    }
}