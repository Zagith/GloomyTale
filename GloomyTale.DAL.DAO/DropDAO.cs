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
    public class DropDAO : MappingBaseDao<Drop, DropDTO>, IDropDAO
    {
        public DropDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public void Insert(List<DropDTO> drops)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (DropDTO Drop in drops)
                    {
                        var entity = _mapper.Map<Drop>(Drop);
                        context.Drop.Add(entity);
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        public DropDTO Insert(DropDTO drop)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<Drop>(drop);
                    context.Drop.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<DropDTO>(drop);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public List<DropDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.Drop.ToList().Select(d => _mapper.Map<DropDTO>(d)).ToList();
            }
        }

        public IEnumerable<DropDTO> LoadByMonster(short monsterVNum)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Drop Drop in context.Drop.Where(s => s.MonsterVNum == monsterVNum || s.MonsterVNum == null))
                {
                    yield return _mapper.Map<DropDTO>(Drop);
                }
            }
        }

        #endregion
    }
}