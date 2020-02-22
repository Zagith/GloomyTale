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
    public class I18NNpcMonsterDAO : MappingBaseDao<I18NNpcMonster, I18NNpcMonsterDto>, II18NNpcMonsterDAO
    {
        public I18NNpcMonsterDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public IEnumerable<I18NNpcMonsterDto> FindByName(string name)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (I18NNpcMonster i18NNpcMonster in context.I18NNpcMonster.Where(s => s.Key.Contains(name)))
                {
                    yield return _mapper.Map<I18NNpcMonsterDto>(i18NNpcMonster);
                }
            }
        }
        public void Insert(List<I18NNpcMonsterDto> skills)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    
                    foreach (I18NNpcMonsterDto skill in skills)
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

        public SaveResult InsertOrUpdate(I18NNpcMonsterDto skill)
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

        public I18NNpcMonsterDto Insert(I18NNpcMonsterDto I18NNpcMonster)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<I18NNpcMonster>(I18NNpcMonster);
                    context.I18NNpcMonster.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<I18NNpcMonsterDto>(I18NNpcMonster);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<I18NNpcMonsterDto> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (I18NNpcMonster i18NNpcMonster in context.I18NNpcMonster)
                {
                    yield return _mapper.Map<I18NNpcMonsterDto>(i18NNpcMonster);
                }
            }
        }

        public I18NNpcMonsterDto LoadById(short I18NNpcMonsterId)
        {

            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    I18NNpcMonster i18NNpcMonster = context.I18NNpcMonster.FirstOrDefault(a => a.I18NNpcMonsterId.Equals(I18NNpcMonsterId));
                    if (i18NNpcMonster != null)
                    {
                        return _mapper.Map<I18NNpcMonsterDto>(i18NNpcMonster);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
            return null;
        }

        private I18NNpcMonsterDto insert(I18NNpcMonsterDto I18NNpcMonster, OpenNosContext context)
        {
            var entity = _mapper.Map<I18NNpcMonster>(I18NNpcMonster);
            context.I18NNpcMonster.Add(entity);
            context.SaveChanges();
            return _mapper.Map<I18NNpcMonsterDto>(entity);
        }

        private I18NNpcMonsterDto update(I18NNpcMonster entity, I18NNpcMonsterDto i18NNpcMonster, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(i18NNpcMonster, entity);
                context.SaveChanges();
            }

            return _mapper.Map<I18NNpcMonsterDto>(entity);
        }

        #endregion
    }
}
