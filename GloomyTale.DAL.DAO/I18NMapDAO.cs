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
    public class I18NMapDAO : II18NMapDAO
    {
        #region Methods

        public void Insert(List<II18NMapDto> skills)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    
                    foreach (II18NMapDto skill in skills)
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

        public SaveResult InsertOrUpdate(II18NMapDto skill)
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

        public II18NMapDto Insert(II18NMapDto I18NMap)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    I18NMapPointData entity = new I18NMapPointData();
                    Mapper.Mappers.I18NMapMapper.ToI18NMap(I18NMap, entity); context.I18NMapPointData.Add(entity);
                    context.SaveChanges();
                    if (Mapper.Mappers.I18NMapMapper.ToI18NMapDTO(entity, I18NMap))
                    {
                        return I18NMap;
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

        public IEnumerable<II18NMapDto> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<II18NMapDto> result = new List<II18NMapDto>();
                foreach (I18NMapPointData I18NMap in context.I18NMapPointData)
                {
                    II18NMapDto dto = new II18NMapDto();
                    Mapper.Mappers.I18NMapMapper.ToI18NMapDTO(I18NMap, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public II18NMapDto LoadById(short I18NMapId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    II18NMapDto dto = new II18NMapDto();
                    if (Mapper.Mappers.I18NMapMapper.ToI18NMapDTO(context.I18NMapPointData.FirstOrDefault(s => s.I18NMapPointDataId.Equals(I18NMapId)), dto))
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

        private static II18NMapDto insert(II18NMapDto I18NMap, OpenNosContext context)
        {
            I18NMapPointData entity = new I18NMapPointData();
            Mapper.Mappers.I18NMapMapper.ToI18NMap(I18NMap, entity);
            context.I18NMapPointData.Add(entity);
            context.SaveChanges();
            if (Mapper.Mappers.I18NMapMapper.ToI18NMapDTO(entity, I18NMap))
            {
                return I18NMap;
            }

            return null;
        }

        private static II18NMapDto update(I18NMapPointData entity, II18NMapDto skill, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mappers.I18NMapMapper.ToI18NMap(skill, entity);
                context.SaveChanges();
            }

            if (Mapper.Mappers.I18NMapMapper.ToI18NMapDTO(entity, skill))
            {
                return skill;
            }

            return null;
        }

        #endregion
    }
}
