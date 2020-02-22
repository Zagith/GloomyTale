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
    public class TeleporterDAO : MappingBaseDao<Teleporter, TeleporterDTO>, ITeleporterDAO
    {
        public TeleporterDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public TeleporterDTO Insert(TeleporterDTO teleporter)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<Teleporter>(teleporter);
                    context.Teleporter.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<TeleporterDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<TeleporterDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Teleporter entity in context.Teleporter)
                {
                    yield return _mapper.Map<TeleporterDTO>(entity);
                }
            }
        }

        public TeleporterDTO LoadById(short teleporterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<TeleporterDTO>(context.Teleporter.FirstOrDefault(i => i.TeleporterId.Equals(teleporterId)));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<TeleporterDTO> LoadFromNpc(int npcId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Teleporter entity in context.Teleporter.Where(c => c.MapNpcId.Equals(npcId)))
                {
                    yield return _mapper.Map<TeleporterDTO>(entity);
                }
            }
        }

        #endregion
    }
}