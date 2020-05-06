using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Entities;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data.Achievements;
using OpenNos.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.DAL.DAO
{
    public class CharacterAchievementDAO : ICharacterAchievementDAO
    {
        public DeleteResult Delete(long characterId, long questId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    CharacterAchievement charQuest = context.CharacterAchievements.FirstOrDefault(i => i.CharacterId == characterId && i.AchievementId == questId);
                    if (charQuest != null)
                    {
                        context.CharacterAchievements.Remove(charQuest);
                        context.SaveChanges();
                    }
                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return DeleteResult.Error;
            }
        }

        public CharacterAchievementDTO InsertOrUpdate(CharacterAchievementDTO charQuest)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return InsertOrUpdate(context, charQuest);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Message: {e.Message}", e);
                return null;
            }
        }

        protected static CharacterAchievementDTO InsertOrUpdate(OpenNosContext context, CharacterAchievementDTO dto)
        {
            Guid primaryKey = dto.Id;
            CharacterAchievement entity = context.Set<CharacterAchievement>().FirstOrDefault(c => c.Id == primaryKey);
            if (entity == null)
            {
                return Insert(dto, context);
            }
            else
            {
                return Update(entity, dto, context);
            }
        }

        private static CharacterAchievementDTO Insert(CharacterAchievementDTO charQuest, OpenNosContext context)
        {
            CharacterAchievement entity = new CharacterAchievement();
            Mapper.Mappers.CharacterAchievementMapper.ToCharacterAchievement(charQuest, entity);
            context.CharacterAchievements.Add(entity);
            context.SaveChanges();
            if (Mapper.Mappers.CharacterAchievementMapper.ToCharacterAchievementDTO(entity, charQuest))
            {
                return charQuest;
            }

            return null;
        }

        private static CharacterAchievementDTO Update(CharacterAchievement entity, CharacterAchievementDTO charQuest, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mappers.CharacterAchievementMapper.ToCharacterAchievement(charQuest, entity);
                context.SaveChanges();
            }

            if (Mapper.Mappers.CharacterAchievementMapper.ToCharacterAchievementDTO(entity, charQuest))
            {
                return charQuest;
            }

            return null;
        }

        public IEnumerable<CharacterAchievementDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<CharacterAchievementDTO> result = new List<CharacterAchievementDTO>();
                foreach (CharacterAchievement charQuest in context.CharacterAchievements.Where(s => s.CharacterId == characterId))
                {
                    CharacterAchievementDTO dto = new CharacterAchievementDTO();
                    Mapper.Mappers.CharacterAchievementMapper.ToCharacterAchievementDTO(charQuest, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public IEnumerable<Guid> LoadKeysByCharacterId(long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return context.CharacterAchievements.Where(i => i.CharacterId == characterId).Select(c => c.Id).ToList();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }
    }
}
