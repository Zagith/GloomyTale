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
    public class CharacterDAO : MappingBaseDao<Character, CharacterDTO>, ICharacterDAO
    {
        public CharacterDAO(IMapper mapper) : base (mapper)
        { }

        #region Methods

        public DeleteResult DeleteByPrimaryKey(long accountId, byte characterSlot)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    // actually a Character wont be deleted, it just will be disabled for future traces
                    Character character = context.Character.SingleOrDefault(c => c.AccountId.Equals(accountId) && c.Slot.Equals(characterSlot) && c.State.Equals((byte)CharacterState.Active));

                    if (character != null)
                    {
                        character.State = (byte)CharacterState.Inactive;
                        character.Name = $"[DELETED]{character.Name}";
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_CHARACTER_ERROR"), characterSlot, e.Message), e);
                return DeleteResult.Error;
            }
        }

        /// <summary>
        /// Returns first 30 occurences of highest Compliment
        /// </summary>
        /// <returns></returns>
        public List<CharacterDTO> GetTopCompliment()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.Character.Where(c => c.State == (byte)CharacterState.Active && c.Account.Authority == AuthorityType.User && !c.Account.PenaltyLog.Any(l => l.Penalty == PenaltyType.Banned && l.DateEnd > DateTime.Now)).OrderByDescending(c => c.Compliment).Take(30).ToList().Select(c => _mapper.Map<CharacterDTO>(c))
                    .ToList();
            }
        }

        /// <summary>
        /// Returns first 30 occurences of highest Act4Points
        /// </summary>
        /// <returns></returns>
        public List<CharacterDTO> GetTopPoints()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.Character.Where(c => c.State == (byte)CharacterState.Active && c.Account.Authority == AuthorityType.User && !c.Account.PenaltyLog.Any(l => l.Penalty == PenaltyType.Banned && l.DateEnd > DateTime.Now)).OrderByDescending(c => c.Act4Points).Take(30).ToList().Select(c => _mapper.Map<CharacterDTO>(c))
                    .ToList();
            }
        }

        /// <summary>
        /// Returns first 30 occurences of highest Reputation
        /// </summary>
        /// <returns></returns>
        public List<CharacterDTO> GetTopReputation()
        {            
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.Character.Where(c => c.State == (byte)CharacterState.Active && c.Account.Authority == AuthorityType.User && !c.Account.PenaltyLog.Any(l => l.Penalty == PenaltyType.Banned && l.DateEnd > DateTime.Now)).OrderByDescending(c => c.Reputation).Take(43).ToList().Select(c => _mapper.Map<CharacterDTO>(c))
                    .ToList();
            }
        }

        public SaveResult InsertOrUpdate(ref CharacterDTO character)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long characterId = character.CharacterId;
                    Character entity = context.Character.FirstOrDefault(c => c.CharacterId.Equals(characterId));
                    if (entity == null)
                    {
                        character = insert(character, context);
                        return SaveResult.Inserted;
                    }
                    character = update(entity, character, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("INSERT_ERROR"), character, e.Message), e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<CharacterDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Character chara in context.Character)
                {
                    yield return _mapper.Map<CharacterDTO>(chara);
                }
            }
        }

        public IEnumerable<CharacterDTO> LoadAllByAccount(long accountId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.Character.Where(c => c.AccountId.Equals(accountId) && c.State.Equals((byte)CharacterState.Active)).OrderByDescending(c => c.Slot).ToList()
                    .Select(c => _mapper.Map<CharacterDTO>(c)).ToList();
            }
        }

        public IEnumerable<CharacterDTO> LoadByAccount(long accountId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.Character.Where(c => c.AccountId.Equals(accountId)).OrderByDescending(c => c.Slot).ToList().Select(c => _mapper.Map<CharacterDTO>(c)).ToList();
            }
        }

        public CharacterDTO LoadById(long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<CharacterDTO>(context.Character.FirstOrDefault(c => c.CharacterId.Equals(characterId)));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public CharacterDTO LoadByName(string name)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<CharacterDTO>(context.Character.SingleOrDefault(c => c.Name.Equals(name) && c.State.Equals((byte)CharacterState.Active)));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
            return null;
        }

        public CharacterDTO LoadBySlot(long accountId, byte slot)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<CharacterDTO>(context.Character.SingleOrDefault(c => c.AccountId.Equals(accountId) && c.Slot.Equals(slot) && c.State.Equals((byte)CharacterState.Active)));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error($"There should be only 1 character per slot, AccountId: {accountId} Slot: {slot}", e);
                return null;
            }
        }

        private CharacterDTO insert(CharacterDTO character, OpenNosContext context)
        {
            var entity = _mapper.Map<Character>(character);
            context.Character.Add(entity);
            context.SaveChanges();
            return _mapper.Map<CharacterDTO>(entity);
        }

        private CharacterDTO update(Character entity, CharacterDTO character, OpenNosContext context)
        {
            if (entity == null)
            {
                return null;
            }

            _mapper.Map(character, entity);
            context.SaveChanges();

            return _mapper.Map<CharacterDTO>(entity);
        }

        #endregion
    }
}