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
using GloomyTale.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace GloomyTale.DAL.DAO
{
    public class CharacterSkillDAO : SynchronizableBaseDAO<CharacterSkill, CharacterSkillDTO>, ICharacterSkillDAO
    {
        public CharacterSkillDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public DeleteResult Delete(long characterId, short skillVNum)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    CharacterSkill invItem = context.CharacterSkill.FirstOrDefault(i => i.CharacterId == characterId && i.SkillVNum == skillVNum);
                    if (invItem != null)
                    {
                        context.CharacterSkill.Remove(invItem);
                        context.SaveChanges();
                    }
                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return DeleteResult.Error;
            }
        }

        public IEnumerable<CharacterSkillDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.CharacterSkill.Where(s => s.CharacterId == characterId).ToArray().Select(_mapper.Map<CharacterSkillDTO>);
            }
        }

        public IEnumerable<Guid> LoadKeysByCharacterId(long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return context.CharacterSkill.Where(i => i.CharacterId == characterId).Select(c => c.Id).ToList();
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