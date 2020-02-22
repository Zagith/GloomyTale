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
    public class I18NMapDAO : MappingBaseDao<I18NMapPointData, I18NMapPointDataDto>, II18NMapDAO
    {
        public I18NMapDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public void Insert(List<I18NMapPointDataDto> skills)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    
                    foreach (I18NMapPointDataDto skill in skills)
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

        public SaveResult InsertOrUpdate(I18NMapPointDataDto skill)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long SkillVNum = skill.I18NMapId;
                    I18NMapPointData entity = context.I18NMapPointData.FirstOrDefault(c => c.I18NMapPointDataId == SkillVNum);

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
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_I18NMap_ERROR"), skill.RegionType, e.Message), e);
                return SaveResult.Error;
            }
        }

        public I18NMapPointDataDto Insert(I18NMapPointDataDto I18NMapPointData)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<I18NMapPointData>(I18NMapPointData);
                    context.I18NMapPointData.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<I18NMapPointDataDto>(I18NMapPointData);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<I18NMapPointDataDto> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (I18NMapPointData i18NMapPointData in context.I18NMapPointData)
                {
                    yield return _mapper.Map<I18NMapPointDataDto>(i18NMapPointData);
                }
            }
        }

        public I18NMapPointDataDto LoadById(short I18NMapPointDataId)
        {

            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    I18NMapPointData i18NMapPointData = context.I18NMapPointData.FirstOrDefault(a => a.I18NMapPointDataId.Equals(I18NMapPointDataId));
                    if (i18NMapPointData != null)
                    {
                        return _mapper.Map<I18NMapPointDataDto>(i18NMapPointData);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
            return null;
        }

        private I18NMapPointDataDto insert(I18NMapPointDataDto I18NMapPointData, OpenNosContext context)
        {
            var entity = _mapper.Map<I18NMapPointData>(I18NMapPointData);
            context.I18NMapPointData.Add(entity);
            context.SaveChanges();
            return _mapper.Map<I18NMapPointDataDto>(entity);
        }

        private I18NMapPointDataDto update(I18NMapPointData entity, I18NMapPointDataDto i18NMapPointData, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(i18NMapPointData, entity);
                context.SaveChanges();
            }

            return _mapper.Map<I18NMapPointDataDto>(entity);
        }

        #endregion
    }
}
