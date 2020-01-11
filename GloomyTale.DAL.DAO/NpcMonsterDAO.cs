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

namespace GloomyTale.DAL.DAO
{
    public class NpcMonsterDAO : INpcMonsterDAO
    {
        #region Methods

        public NpcMonsterDTO LoadByKey(string vNum)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    NpcMonsterDTO dto = new NpcMonsterDTO();
                    if (Mapper.Mappers.NpcMonsterMapper.ToNpcMonsterDTO(context.NpcMonster.FirstOrDefault(i => i.Name.Equals(vNum)), dto))
                    {
                        return dto;
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

        public IEnumerable<NpcMonsterDTO> FindByName(string name)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<NpcMonsterDTO> result = new List<NpcMonsterDTO>();
                foreach (NpcMonster npcMonster in context.NpcMonster.Where(s => string.IsNullOrEmpty(name) ? s.Name.Equals("") : s.Name.Contains(name)))
                {
                    NpcMonsterDTO dto = new NpcMonsterDTO();
                    Mapper.Mappers.NpcMonsterMapper.ToNpcMonsterDTO(npcMonster, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public void Insert(List<NpcMonsterDTO> npcMonsters)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    
                    foreach (NpcMonsterDTO Item in npcMonsters)
                    {
                        NpcMonster entity = new NpcMonster();
                        Mapper.Mappers.NpcMonsterMapper.ToNpcMonster(Item, entity);
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

        public NpcMonsterDTO Insert(NpcMonsterDTO npc)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    NpcMonster entity = new NpcMonster();
                    Mapper.Mappers.NpcMonsterMapper.ToNpcMonster(npc, entity);
                    context.NpcMonster.Add(entity);
                    context.SaveChanges();
                    if (Mapper.Mappers.NpcMonsterMapper.ToNpcMonsterDTO(entity, npc))
                    {
                        return npc;
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
                List<NpcMonsterDTO> result = new List<NpcMonsterDTO>();
                foreach (NpcMonster NpcMonster in context.NpcMonster)
                {
                    NpcMonsterDTO dto = new NpcMonsterDTO();
                    Mapper.Mappers.NpcMonsterMapper.ToNpcMonsterDTO(NpcMonster, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public NpcMonsterDTO LoadByVNum(short npcMonsterVNum)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    NpcMonsterDTO dto = new NpcMonsterDTO();
                    if (Mapper.Mappers.NpcMonsterMapper.ToNpcMonsterDTO(context.NpcMonster.FirstOrDefault(i => i.NpcMonsterVNum.Equals(npcMonsterVNum)), dto))
                    {
                        return dto;
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

        private static NpcMonsterDTO insert(NpcMonsterDTO npcMonster, OpenNosContext context)
        {
            NpcMonster entity = new NpcMonster();
            Mapper.Mappers.NpcMonsterMapper.ToNpcMonster(npcMonster, entity);
            context.NpcMonster.Add(entity);
            context.SaveChanges();
            if (Mapper.Mappers.NpcMonsterMapper.ToNpcMonsterDTO(entity, npcMonster))
            {
                return npcMonster;
            }

            return null;
        }

        private static NpcMonsterDTO update(NpcMonster entity, NpcMonsterDTO npcMonster, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mappers.NpcMonsterMapper.ToNpcMonster(npcMonster, entity);
                context.SaveChanges();
            }
            if (Mapper.Mappers.NpcMonsterMapper.ToNpcMonsterDTO(entity, npcMonster))
            {
                return npcMonster;
            }

            return null;
        }

        #endregion
    }
}