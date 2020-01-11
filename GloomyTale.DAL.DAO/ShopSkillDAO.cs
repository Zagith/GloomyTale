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

namespace GloomyTale.DAL.DAO
{
    public class ShopSkillDAO : IShopSkillDAO
    {
        #region Methods

        public ShopSkillDTO Insert(ShopSkillDTO shopSkill)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    ShopSkill entity = new ShopSkill();
                    Mapper.Mappers.ShopSkillMapper.ToShopSkill(shopSkill, entity);
                    context.ShopSkill.Add(entity);
                    context.SaveChanges();
                    if (Mapper.Mappers.ShopSkillMapper.ToShopSkillDTO(entity, shopSkill))
                    {
                        return shopSkill;
                    }

                    return null;
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
                        ShopSkill entity = new ShopSkill();
                        Mapper.Mappers.ShopSkillMapper.ToShopSkill(Skill, entity);
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
                List<ShopSkillDTO> result = new List<ShopSkillDTO>();
                foreach (ShopSkill entity in context.ShopSkill)
                {
                    ShopSkillDTO dto = new ShopSkillDTO();
                    Mapper.Mappers.ShopSkillMapper.ToShopSkillDTO(entity, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public IEnumerable<ShopSkillDTO> LoadByShopId(int shopId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<ShopSkillDTO> result = new List<ShopSkillDTO>();
                foreach (ShopSkill ShopSkill in context.ShopSkill.Where(s => s.ShopId.Equals(shopId)))
                {
                    ShopSkillDTO dto = new ShopSkillDTO();
                    Mapper.Mappers.ShopSkillMapper.ToShopSkillDTO(ShopSkill, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        #endregion
    }
}