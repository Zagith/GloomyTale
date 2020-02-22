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
    public class FamilyCharacterDAO : MappingBaseDao<FamilyCharacter, FamilyCharacterDTO>, IFamilyCharacterDAO
    {
        public FamilyCharacterDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public DeleteResult Delete(long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    FamilyCharacter familyCharacter = context.FamilyCharacter.FirstOrDefault(c => c.CharacterId == characterId);

                    if (familyCharacter == null)
                    {
                        return DeleteResult.NotFound;
                    }

                    context.FamilyCharacter.Remove(familyCharacter);
                    context.SaveChanges();

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_FAMILYCHARACTER_ERROR"), e.Message), e);
                return DeleteResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(ref FamilyCharacterDTO character)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long familyCharacterId = character.FamilyCharacterId;
                    FamilyCharacter entity = context.FamilyCharacter.FirstOrDefault(c => c.FamilyCharacterId.Equals(familyCharacterId));

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

        public FamilyCharacterDTO LoadByCharacterId(long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<FamilyCharacterDTO>(context.FamilyCharacter.FirstOrDefault(c => c.CharacterId == characterId));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IList<FamilyCharacterDTO> LoadByFamilyId(long familyId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.FamilyCharacter.Where(fc => fc.FamilyId.Equals(familyId)).ToList().Select(c => _mapper.Map<FamilyCharacterDTO>(c)).ToList();
            }
        }

        public FamilyCharacterDTO LoadById(long familyCharacterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<FamilyCharacterDTO>(context.FamilyCharacter.FirstOrDefault(c => c.FamilyCharacterId.Equals(familyCharacterId)));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        private FamilyCharacterDTO insert(FamilyCharacterDTO character, OpenNosContext context)
        {
            var entity = _mapper.Map<FamilyCharacter>(character);
            context.FamilyCharacter.Add(entity);
            context.SaveChanges();
            return _mapper.Map<FamilyCharacterDTO>(entity);
        }

        private FamilyCharacterDTO update(FamilyCharacter entity, FamilyCharacterDTO character, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(character, entity);
                context.SaveChanges();
            }

            return _mapper.Map<FamilyCharacterDTO>(entity);
        }

        #endregion
    }
}