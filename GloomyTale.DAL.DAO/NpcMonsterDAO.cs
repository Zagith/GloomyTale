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
    public class NpcMonsterDAO : MappingBaseDao<NpcMonster, NpcMonsterDTO>, INpcMonsterDAO
    {
        public NpcMonsterDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public NpcMonsterDTO LoadByKey(string vnum)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<NpcMonsterDTO>(context.NpcMonster.FirstOrDefault(i => i.Name.Equals(vnum)));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<NpcMonsterDTO> FindByName(string name)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (NpcMonster npcMonster in context.NpcMonster.Where(s => s.Name.Contains(name)))
                {
                    yield return _mapper.Map<NpcMonsterDTO>(npcMonster);
                }
            }
        }

        public void Insert(List<NpcMonsterDTO> npcMonsters)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {

                    foreach (NpcMonsterDTO npc in npcMonsters)
                    {
                        var entity = _mapper.Map<NpcMonster>(npc);
                        context.NpcMonster.Add(entity);
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        public SaveResult InsertOrUpdate(ref NpcMonsterDTO npcMonster)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    short npcMonsterVNum = npcMonster.NpcMonsterVNum;
                    NpcMonster entity = context.NpcMonster.FirstOrDefault(c => c.NpcMonsterVNum.Equals(npcMonsterVNum));

                    if (entity == null)
                    {
                        npcMonster = insert(npcMonster, context);
                        return SaveResult.Inserted;
                    }

                    npcMonster = update(entity, npcMonster, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_NPCMONSTER_ERROR"), npcMonster.NpcMonsterVNum, e.Message), e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<NpcMonsterDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (NpcMonster NpcMonster in context.NpcMonster)
                {
                    yield return _mapper.Map<NpcMonsterDTO>(NpcMonster);
                }
            }
        }

        public NpcMonsterDTO LoadByVNum(short npcMonsterVNum)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<NpcMonsterDTO>(context.NpcMonster.FirstOrDefault(i => i.NpcMonsterVNum.Equals(npcMonsterVNum)));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        private NpcMonsterDTO insert(NpcMonsterDTO npcMonster, OpenNosContext context)
        {
            var entity = _mapper.Map<NpcMonster>(npcMonster);
            context.NpcMonster.Add(entity);
            context.SaveChanges();
            return _mapper.Map<NpcMonsterDTO>(entity);
        }

        private NpcMonsterDTO update(NpcMonster entity, NpcMonsterDTO npcMonster, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(npcMonster, entity);
                context.SaveChanges();
            }

            return _mapper.Map<NpcMonsterDTO>(entity);
        }

        #endregion
    }
}