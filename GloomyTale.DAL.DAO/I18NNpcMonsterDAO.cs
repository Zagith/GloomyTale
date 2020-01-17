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
using AutoMapper;

namespace GloomyTale.DAL.DAO
{
    public class I18NNpcMonsterDAO : MappingBaseDao<I18NNpcMonster, II18NNpcMonsterDto>, II18NNpcMonsterDAO
    {
        public I18NNpcMonsterDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public IEnumerable<II18NNpcMonsterDto> FindByName(string name)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<II18NNpcMonsterDto> result = new List<II18NNpcMonsterDto>();
                foreach (I18NNpcMonster item in context.I18NNpcMonster.Where(s => string.IsNullOrEmpty(name) ? s.Text.Equals("") : s.Text.Contains(name)))
                {
                    II18NNpcMonsterDto dto = new II18NNpcMonsterDto();
                    Mapper.Mappers.I18NNpcMonsterMapper.ToI18NNpcMonsterDTO(item, dto);
                    result.Add(dto);
                }
                return result;
            }
        }
        public void Insert(List<II18NNpcMonsterDto> skills)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    
                    foreach (II18NNpcMonsterDto skill in skills)
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

        public SaveResult InsertOrUpdate(II18NNpcMonsterDto skill)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long SkillVNum = skill.I18NNpcMonsterId;
                    I18NNpcMonster entity = context.I18NNpcMonster.FirstOrDefault(c => c.I18NNpcMonsterId == SkillVNum);

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
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_I18NNpcMonster_ERROR"), skill.RegionType, e.Message), e);
                return SaveResult.Error;
            }
        }

        public II18NNpcMonsterDto Insert(II18NNpcMonsterDto I18NNpcMonster)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    I18NNpcMonster entity = new I18NNpcMonster();
                    Mapper.Mappers.I18NNpcMonsterMapper.ToI18NNpcMonster(I18NNpcMonster, entity); context.I18NNpcMonster.Add(entity);
                    context.SaveChanges();
                    if (Mapper.Mappers.I18NNpcMonsterMapper.ToI18NNpcMonsterDTO(entity, I18NNpcMonster))
                    {
                        return I18NNpcMonster;
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

        public IEnumerable<II18NNpcMonsterDto> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<II18NNpcMonsterDto> result = new List<II18NNpcMonsterDto>();
                foreach (I18NNpcMonster I18NNpcMonster in context.I18NNpcMonster)
                {
                    II18NNpcMonsterDto dto = new II18NNpcMonsterDto();
                    Mapper.Mappers.I18NNpcMonsterMapper.ToI18NNpcMonsterDTO(I18NNpcMonster, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public II18NNpcMonsterDto LoadById(short I18NNpcMonsterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    II18NNpcMonsterDto dto = new II18NNpcMonsterDto();
                    if (Mapper.Mappers.I18NNpcMonsterMapper.ToI18NNpcMonsterDTO(context.I18NNpcMonster.FirstOrDefault(s => s.I18NNpcMonsterId.Equals(I18NNpcMonsterId)), dto))
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

        private static II18NNpcMonsterDto insert(II18NNpcMonsterDto I18NNpcMonster, OpenNosContext context)
        {
            I18NNpcMonster entity = new I18NNpcMonster();
            Mapper.Mappers.I18NNpcMonsterMapper.ToI18NNpcMonster(I18NNpcMonster, entity);
            context.I18NNpcMonster.Add(entity);
            context.SaveChanges();
            if (Mapper.Mappers.I18NNpcMonsterMapper.ToI18NNpcMonsterDTO(entity, I18NNpcMonster))
            {
                return I18NNpcMonster;
            }

            return null;
        }

        private static II18NNpcMonsterDto update(I18NNpcMonster entity, II18NNpcMonsterDto skill, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mappers.I18NNpcMonsterMapper.ToI18NNpcMonster(skill, entity);
                context.SaveChanges();
            }

            if (Mapper.Mappers.I18NNpcMonsterMapper.ToI18NNpcMonsterDTO(entity, skill))
            {
                return skill;
            }

            return null;
        }

        #endregion
    }
}