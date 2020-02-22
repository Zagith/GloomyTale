using System;
using System.Collections.Generic;
using System.Linq;
using GloomyTale.Core;
using GloomyTale.DAL.EF;
using GloomyTale.DAL.Interface;
using GloomyTale.Data;
using GloomyTale.Data.Enums;
using GloomyTale.DAL.EF.Helpers;
using AutoMapper;

namespace GloomyTale.DAL.DAO
{
    public class CharacterQuestDAO : SynchronizableBaseDAO<CharacterQuest, CharacterQuestDTO>, ICharacterQuestDAO
    {
        public CharacterQuestDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public DeleteResult Delete(long characterId, long questId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    CharacterQuest charQuest = context.CharacterQuest.FirstOrDefault(i => i.CharacterId == characterId && i.QuestId == questId);
                    if (charQuest != null)
                    {
                        context.CharacterQuest.Remove(charQuest);
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

        public IEnumerable<CharacterQuestDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (CharacterQuest entity in context.CharacterQuest.Where(i => i.CharacterId == characterId))
                {
                    yield return _mapper.Map<CharacterQuestDTO>(entity);
                }
            }
        }

        public IEnumerable<Guid> LoadKeysByCharacterId(long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return context.CharacterQuest.Where(i => i.CharacterId == characterId).Select(c => c.Id).ToList();
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