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
    public class I18NItemDAO : MappingBaseDao<I18NItem, I18NItemDto>, II18NItemDAO
    {
        public I18NItemDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public IEnumerable<I18NItemDto> FindByName(string name)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (I18NItem i18NItem in context.I18NItem.Where(s => s.Key.Contains(name)))
                {
                    yield return _mapper.Map<I18NItemDto>(i18NItem);
                }
            }
        }

        public void Insert(List<I18NItemDto> skills)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    
                    foreach (I18NItemDto skill in skills)
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

        public SaveResult InsertOrUpdate(I18NItemDto skill)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long SkillVNum = skill.I18NItemId;
                    I18NItem entity = context.I18NItem.FirstOrDefault(c => c.I18NItemId == SkillVNum);

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
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_I18NITEM_ERROR"), skill.RegionType, e.Message), e);
                return SaveResult.Error;
            }
        }

        public I18NItemDto Insert(I18NItemDto I18NItem)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<I18NItem>(I18NItem);
                    context.I18NItem.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<I18NItemDto>(I18NItem);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<I18NItemDto> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (I18NItem i18NItem in context.I18NItem)
                {
                    yield return _mapper.Map<I18NItemDto>(i18NItem);
                }
            }
        }

        public I18NItemDto LoadById(short I18NItemId)
        {

            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    I18NItem i18NItem = context.I18NItem.FirstOrDefault(a => a.I18NItemId.Equals(I18NItemId));
                    if (i18NItem != null)
                    {
                        return _mapper.Map<I18NItemDto>(i18NItem);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
            return null;
        }

        private I18NItemDto insert(I18NItemDto I18NItem, OpenNosContext context)
        {
            var entity = _mapper.Map<I18NItem>(I18NItem);
            context.I18NItem.Add(entity);
            context.SaveChanges();
            return _mapper.Map<I18NItemDto>(entity);
        }

        private I18NItemDto update(I18NItem entity, I18NItemDto i18NItem, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(i18NItem, entity);
                context.SaveChanges();
            }

            return _mapper.Map<I18NItemDto>(entity);
        }

        #endregion
    }
}
