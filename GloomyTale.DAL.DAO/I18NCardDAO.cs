using GloomyTale.Core;
using GloomyTale.DAL.EF;
using GloomyTale.DAL.EF.Entities;
using GloomyTale.DAL.EF.Helpers;
using GloomyTale.DAL.Interface;
using GloomyTale.Data.Enums;
using GloomyTale.Data.I18N;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace GloomyTale.DAL.DAO
{
    public class I18NCardDAO : MappingBaseDao<I18NCard, I18NCardDto>, II18NCardDAO
    {
        public I18NCardDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public void Insert(List<I18NCardDto> skills)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    
                    foreach (I18NCardDto skill in skills)
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

        public SaveResult InsertOrUpdate(I18NCardDto skill)
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

        public I18NCardDto Insert(I18NCardDto I18NCard)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<I18NCard>(I18NCard);
                    context.I18NCard.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<I18NCardDto>(I18NCard);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<I18NCardDto> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (I18NCard i18NCard in context.I18NCard)
                {
                    yield return _mapper.Map<I18NCardDto>(i18NCard);
                }
            }
        }

        public I18NCardDto LoadById(short I18NCardId)
        {

            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    I18NCard i18NCard = context.I18NCard.FirstOrDefault(a => a.I18NCardId.Equals(I18NCardId));
                    if (i18NCard != null)
                    {
                        return _mapper.Map<I18NCardDto>(i18NCard);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
            return null;
        }

        private I18NCardDto insert(I18NCardDto I18NCard, OpenNosContext context)
        {
            var entity = _mapper.Map<I18NCard>(I18NCard);
            context.I18NCard.Add(entity);
            context.SaveChanges();
            return _mapper.Map<I18NCardDto>(entity);
        }

        private I18NCardDto update(I18NCard entity, I18NCardDto i18NCard, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(i18NCard, entity);
                context.SaveChanges();
            }

            return _mapper.Map<I18NCardDto>(entity);
        }

        #endregion
    }
}
