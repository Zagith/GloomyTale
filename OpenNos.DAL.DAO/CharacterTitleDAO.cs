using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using OpenNos.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.DAO
{
    public class CharacterTitleDAO : ICharacterTitlesDAO
    {
        #region Methods
               
        public SaveResult InsertOrUpdate(ref CharacterTitleDTO CharacterTitle)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long characterId = CharacterTitle.CharacterId;
                    long titleId = CharacterTitle.TitleId;
                    CharacterTitle entity = context.CharacterTitle.FirstOrDefault(c => c.CharacterId.Equals(characterId) && c.TitleId.Equals(titleId));

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
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_CharacterTitle_ERROR"), CharacterTitle.CharacterTitleId, e.Message), e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<CharacterTitleDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<CharacterTitleDTO> result = new List<CharacterTitleDTO>();
                foreach (CharacterTitle entity in context.CharacterTitle)
                {
                    CharacterTitleDTO dto = new CharacterTitleDTO();
                    Mapper.Mappers.CharacterTitleMapper.ToCharacterTitleDTO(entity, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public CharacterTitleDTO LoadById(long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    CharacterTitleDTO dto = new CharacterTitleDTO();
                    if (Mapper.Mappers.CharacterTitleMapper.ToCharacterTitleDTO(context.CharacterTitle.FirstOrDefault(s => s.CharacterTitleId.Equals(characterId)), dto))
                    {
                        return dto;
                    }

                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private static CharacterTitleDTO insert(CharacterTitleDTO relation, OpenNosContext context)
        {
            CharacterTitle entity = new CharacterTitle();
            Mapper.Mappers.CharacterTitleMapper.ToCharacterTitle(relation, entity);
            context.CharacterTitle.Add(entity);
            context.SaveChanges();
            if (Mapper.Mappers.CharacterTitleMapper.ToCharacterTitleDTO(entity, relation))
            {
                return relation;
            }

            return null;
        }

        private static CharacterTitleDTO update(CharacterTitle entity, CharacterTitleDTO relation, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mappers.CharacterTitleMapper.ToCharacterTitle(relation, entity);
                context.SaveChanges();
            }

            if (Mapper.Mappers.CharacterTitleMapper.ToCharacterTitleDTO(entity, relation))
            {
                return relation;
            }

            return null;
        }

        #endregion
    }
}
