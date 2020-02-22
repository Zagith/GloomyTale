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
    public class ShopSkillDAO : MappingBaseDao<ShopSkill, ShopSkillDTO>, IShopSkillDAO
    {
        public ShopSkillDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public ShopSkillDTO Insert(ShopSkillDTO shopSkill)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<ShopSkill>(shopSkill);
                    context.ShopSkill.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<ShopSkillDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public void Insert(List<ShopSkillDTO> skills)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (ShopSkillDTO Skill in skills)
                    {
                        var entity = _mapper.Map<ShopSkill>(Skill);
                        context.ShopSkill.Add(entity);
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        public IEnumerable<ShopSkillDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (ShopSkill entity in context.ShopSkill)
                {
                    yield return _mapper.Map<ShopSkillDTO>(entity);
                }
            }
        }

        public IEnumerable<ShopSkillDTO> LoadByShopId(int shopId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (ShopSkill ShopSkill in context.ShopSkill.Where(s => s.ShopId.Equals(shopId)))
                {
                    yield return _mapper.Map<ShopSkillDTO>(ShopSkill);
                }
            }
        }

        #endregion
    }
}