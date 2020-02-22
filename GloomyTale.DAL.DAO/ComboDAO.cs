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
    public class ComboDAO : MappingBaseDao<Combo, ComboDTO>, IComboDAO
    {
        public ComboDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public void Insert(List<ComboDTO> combos)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (ComboDTO combo in combos)
                    {
                        var entity = _mapper.Map<Combo>(combo);
                        context.Combo.Add(entity);
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        public ComboDTO Insert(ComboDTO combo)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<Combo>(combo);
                    context.Combo.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<ComboDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<ComboDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Combo combo in context.Combo)
                {
                    yield return _mapper.Map<ComboDTO>(combo);
                }
            }
        }

        public ComboDTO LoadById(short comboId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<ComboDTO>(context.Combo.FirstOrDefault(s => s.SkillVNum.Equals(comboId)));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<ComboDTO> LoadBySkillVnum(short skillVNum)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Combo combo in context.Combo.Where(c => c.SkillVNum == skillVNum))
                {
                    yield return _mapper.Map<ComboDTO>(combo);
                }
            }
        }

        public IEnumerable<ComboDTO> LoadByVNumHitAndEffect(short skillVNum, short hit, short effect)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Combo combo in context.Combo.Where(s => s.SkillVNum == skillVNum && s.Hit == hit && s.Effect == effect))
                {
                    yield return _mapper.Map<ComboDTO>(combo);
                }
            }
        }

        #endregion
    }
}