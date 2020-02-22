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
    public class I18NSkillDAO : MappingBaseDao<I18NSkill, I18NSkillDto>, II18NSkillDAO
    {
        public I18NSkillDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public void Insert(List<I18NSkillDto> skills)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    
                    foreach (I18NSkillDto skill in skills)
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

        public SaveResult InsertOrUpdate(I18NSkillDto skill)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long SkillVNum = skill.I18NSkillId;
                    I18NSkill entity = context.I18NSkill.FirstOrDefault(c => c.I18NSkillId == SkillVNum);

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
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_I18NSkill_ERROR"), skill.RegionType, e.Message), e);
                return SaveResult.Error;
            }
        }

        public I18NSkillDto Insert(I18NSkillDto I18NSkill)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<I18NSkill>(I18NSkill);
                    context.I18NSkill.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<I18NSkillDto>(I18NSkill);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<I18NSkillDto> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (I18NSkill i18NSkill in context.I18NSkill)
                {
                    yield return _mapper.Map<I18NSkillDto>(i18NSkill);
                }
            }
        }

        public I18NSkillDto LoadById(short I18NSkillId)
        {

            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    I18NSkill i18NSkill = context.I18NSkill.FirstOrDefault(a => a.I18NSkillId.Equals(I18NSkillId));
                    if (i18NSkill != null)
                    {
                        return _mapper.Map<I18NSkillDto>(i18NSkill);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
            return null;
        }

        private I18NSkillDto insert(I18NSkillDto I18NSkill, OpenNosContext context)
        {
            var entity = _mapper.Map<I18NSkill>(I18NSkill);
            context.I18NSkill.Add(entity);
            context.SaveChanges();
            return _mapper.Map<I18NSkillDto>(entity);
        }

        private I18NSkillDto update(I18NSkill entity, I18NSkillDto i18NSkill, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(i18NSkill, entity);
                context.SaveChanges();
            }

            return _mapper.Map<I18NSkillDto>(entity);
        }

        #endregion
    }
}
