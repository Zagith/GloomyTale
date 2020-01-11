﻿using GloomyTale.Core;
using GloomyTale.DAL.EF;
using GloomyTale.DAL.EF.Entities;
using GloomyTale.DAL.EF.Helpers;
using GloomyTale.DAL.Interface;
using GloomyTale.Data.Enums;
using GloomyTale.Data.I18N;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GloomyTale.DAL.DAO
{
    public class I18NSkillDAO : II18NSkillDAO
    {
        #region Methods

        public void Insert(List<II18NSkillDto> skills)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    
                    foreach (II18NSkillDto skill in skills)
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

        public SaveResult InsertOrUpdate(II18NSkillDto skill)
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

        public II18NSkillDto Insert(II18NSkillDto I18NSkill)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    I18NSkill entity = new I18NSkill();
                    Mapper.Mappers.I18NSkillMapper.ToI18NSkill(I18NSkill, entity); context.I18NSkill.Add(entity);
                    context.SaveChanges();
                    if (Mapper.Mappers.I18NSkillMapper.ToI18NSkillDTO(entity, I18NSkill))
                    {
                        return I18NSkill;
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

        public IEnumerable<II18NSkillDto> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<II18NSkillDto> result = new List<II18NSkillDto>();
                foreach (I18NSkill I18NSkill in context.I18NSkill)
                {
                    II18NSkillDto dto = new II18NSkillDto();
                    Mapper.Mappers.I18NSkillMapper.ToI18NSkillDTO(I18NSkill, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public II18NSkillDto LoadById(short I18NSkillId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    II18NSkillDto dto = new II18NSkillDto();
                    if (Mapper.Mappers.I18NSkillMapper.ToI18NSkillDTO(context.I18NSkill.FirstOrDefault(s => s.I18NSkillId.Equals(I18NSkillId)), dto))
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

        private static II18NSkillDto insert(II18NSkillDto I18NSkill, OpenNosContext context)
        {
            I18NSkill entity = new I18NSkill();
            Mapper.Mappers.I18NSkillMapper.ToI18NSkill(I18NSkill, entity);
            context.I18NSkill.Add(entity);
            context.SaveChanges();
            if (Mapper.Mappers.I18NSkillMapper.ToI18NSkillDTO(entity, I18NSkill))
            {
                return I18NSkill;
            }

            return null;
        }

        private static II18NSkillDto update(I18NSkill entity, II18NSkillDto skill, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mappers.I18NSkillMapper.ToI18NSkill(skill, entity);
                context.SaveChanges();
            }

            if (Mapper.Mappers.I18NSkillMapper.ToI18NSkillDTO(entity, skill))
            {
                return skill;
            }

            return null;
        }

        #endregion
    }
}
