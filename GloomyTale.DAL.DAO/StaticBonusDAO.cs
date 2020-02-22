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
using GloomyTale.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace GloomyTale.DAL.DAO
{
    public class StaticBonusDAO : MappingBaseDao<StaticBonus, StaticBonusDTO>, IStaticBonusDAO
    {
        public StaticBonusDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public void Delete(short bonusToDelete, long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    StaticBonus bon = context.StaticBonus.FirstOrDefault(c => c.StaticBonusType == (StaticBonusType)bonusToDelete && c.CharacterId == characterId);

                    if (bon != null)
                    {
                        context.StaticBonus.Remove(bon);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_ERROR"), bonusToDelete, e.Message), e);
            }
        }

        public SaveResult InsertOrUpdate(ref StaticBonusDTO staticBonus)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long id = staticBonus.CharacterId;
                    StaticBonusType cardid = staticBonus.StaticBonusType;
                    StaticBonus entity = context.StaticBonus.FirstOrDefault(c => c.StaticBonusType == cardid && c.CharacterId == id);

                    if (entity == null)
                    {
                        staticBonus = insert(staticBonus, context);
                        return SaveResult.Inserted;
                    }
                    staticBonus.StaticBonusId = entity.StaticBonusId;
                    staticBonus = update(entity, staticBonus, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<StaticBonusDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (StaticBonus entity in context.StaticBonus.Where(i => i.CharacterId == characterId && i.DateEnd > DateTime.Now))
                {
                    yield return _mapper.Map<StaticBonusDTO>(entity);
                }
            }
        }

        public StaticBonusDTO LoadById(long sbId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<StaticBonusDTO>(context.RespawnMapType.FirstOrDefault(s => s.RespawnMapTypeId.Equals(sbId)));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<short> LoadTypeByCharacterId(long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return context.StaticBonus.Where(i => i.CharacterId == characterId).Select(qle => (short)qle.StaticBonusType).ToList();
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        private StaticBonusDTO insert(StaticBonusDTO sb, OpenNosContext context)
        {
            try
            {
                var entity = _mapper.Map<StaticBonus>(sb);
                context.StaticBonus.Add(entity);
                context.SaveChanges();
                return _mapper.Map<StaticBonusDTO>(entity);
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        private StaticBonusDTO update(StaticBonus entity, StaticBonusDTO sb, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(sb, entity);
                context.SaveChanges();
            }

            return _mapper.Map<StaticBonusDTO>(entity);
        }

        #endregion
    }
}