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
    public class NpcMonsterSkillDAO : MappingBaseDao<NpcMonsterSkill, NpcMonsterSkillDTO>, INpcMonsterSkillDAO
    {
        public NpcMonsterSkillDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public NpcMonsterSkillDTO Insert(ref NpcMonsterSkillDTO npcMonsterSkill)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<NpcMonsterSkill>(npcMonsterSkill);
                    context.NpcMonsterSkill.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<NpcMonsterSkillDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public void Insert(List<NpcMonsterSkillDTO> skills)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (NpcMonsterSkillDTO Skill in skills)
                    {
                        var entity = _mapper.Map<NpcMonsterSkill>(Skill);
                        context.NpcMonsterSkill.Add(entity);
                    }


                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        public List<NpcMonsterSkillDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.NpcMonsterSkill.ToList().Select(n => _mapper.Map<NpcMonsterSkillDTO>(n)).ToList();
            }
        }

        public IEnumerable<NpcMonsterSkillDTO> LoadByNpcMonster(short npcId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (NpcMonsterSkill NpcMonsterSkillobject in context.NpcMonsterSkill.Where(i => i.NpcMonsterVNum == npcId))
                {
                    yield return _mapper.Map<NpcMonsterSkillDTO>(NpcMonsterSkillobject);
                }
            }
        }

        #endregion
    }
}