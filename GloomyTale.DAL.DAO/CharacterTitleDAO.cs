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
    public class CharacterTitleDAO : MappingBaseDao<CharacterTitle, CharacterTitleDTO>, ICharacterTitlesDAO
    {
        public CharacterTitleDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public SaveResult InsertOrUpdate(ref CharacterTitleDTO CharacterTitle)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long characterId = CharacterTitle.CharacterId;
                    short titleId = CharacterTitle.TitleType;
                    CharacterTitle entity = context.CharacterTitle.FirstOrDefault(c => c.CharacterId.Equals(characterId) && c.TitleType.Equals(titleId));

                    if (entity == null)
                    {
                        CharacterTitle = insert(CharacterTitle, context);
                        return SaveResult.Inserted;
                    }
                    CharacterTitle.CharacterTitleId = entity.CharacterTitleId;
                    CharacterTitle = update(entity, CharacterTitle, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_CharacterTitle_ERROR"), CharacterTitle.CharacterTitleId, e.Message), e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<CharacterTitleDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (CharacterTitle title in context.CharacterTitle)
                {
                    yield return _mapper.Map<CharacterTitleDTO>(title);
                }
            }
        }

        public IEnumerable<CharacterTitleDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.CharacterTitle.Where(s => s.CharacterId == characterId).ToArray().Select(_mapper.Map<CharacterTitleDTO>);
            }
        }

        public CharacterTitleDTO LoadById(long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    CharacterTitle character = context.CharacterTitle.FirstOrDefault(a => a.CharacterId.Equals(characterId));
                    if (character != null)
                    {
                        return _mapper.Map<CharacterTitleDTO>(character);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);                
            }
            return null;
        }

        private CharacterTitleDTO insert(CharacterTitleDTO title, OpenNosContext context)
        {
            var entity = _mapper.Map<CharacterTitle>(title);
            context.CharacterTitle.Add(entity);
            context.SaveChanges();
            return _mapper.Map<CharacterTitleDTO>(entity);
        }

        private CharacterTitleDTO update(CharacterTitle entity, CharacterTitleDTO title, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(title, entity);
                context.SaveChanges();
            }

            return _mapper.Map<CharacterTitleDTO>(entity);
        }

        #endregion
    }
}
