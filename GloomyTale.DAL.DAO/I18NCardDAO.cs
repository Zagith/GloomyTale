using OpenNos.Core;
using GloomyTale.DAL.EF;
using GloomyTale.DAL.EF.Entities;
using GloomyTale.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data.Enums;
using OpenNos.Data.I18N;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.DAL.DAO
{
    public class I18NCardDAO : II18NCardDAO
    {
        #region Methods

        public void Insert(List<II18NCardDto> skills)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    
                    foreach (II18NCardDto skill in skills)
                    {
                        InsertOrUpdate(skill);
                    }
                    
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        public SaveResult InsertOrUpdate(II18NCardDto skill)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long SkillVNum = skill.I18NCardId;
                    I18NCard entity = context.I18NCard.FirstOrDefault(c => c.I18NCardId == SkillVNum);

                    if (entity == null)
                    {
                        skill = insert(skill, context);
                        return SaveResult.Inserted;
                    }

                    skill = update(entity, skill, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_I18NCard_ERROR"), skill.RegionType, e.Message), e);
                return SaveResult.Error;
            }
        }

        public II18NCardDto Insert(II18NCardDto I18NCard)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    I18NCard entity = new I18NCard();
                    Mapper.Mappers.I18NCardMapper.ToI18NCard(I18NCard, entity); context.I18NCard.Add(entity);
                    context.SaveChanges();
                    if (Mapper.Mappers.I18NCardMapper.ToI18NCardDTO(entity, I18NCard))
                    {
                        return I18NCard;
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

        public IEnumerable<II18NCardDto> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<II18NCardDto> result = new List<II18NCardDto>();
                foreach (I18NCard I18NCard in context.I18NCard)
                {
                    II18NCardDto dto = new II18NCardDto();
                    Mapper.Mappers.I18NCardMapper.ToI18NCardDTO(I18NCard, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public II18NCardDto LoadById(short I18NCardId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    II18NCardDto dto = new II18NCardDto();
                    if (Mapper.Mappers.I18NCardMapper.ToI18NCardDTO(context.I18NCard.FirstOrDefault(s => s.I18NCardId.Equals(I18NCardId)), dto))
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

        private static II18NCardDto insert(II18NCardDto I18NCard, OpenNosContext context)
        {
            I18NCard entity = new I18NCard();
            Mapper.Mappers.I18NCardMapper.ToI18NCard(I18NCard, entity);
            context.I18NCard.Add(entity);
            context.SaveChanges();
            if (Mapper.Mappers.I18NCardMapper.ToI18NCardDTO(entity, I18NCard))
            {
                return I18NCard;
            }

            return null;
        }

        private static II18NCardDto update(I18NCard entity, II18NCardDto skill, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mappers.I18NCardMapper.ToI18NCard(skill, entity);
                context.SaveChanges();
            }

            if (Mapper.Mappers.I18NCardMapper.ToI18NCardDTO(entity, skill))
            {
                return skill;
            }

            return null;
        }

        #endregion
    }
}
