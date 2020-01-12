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
using System.Text;
using System.Threading.Tasks;

namespace GloomyTale.DAL.DAO
{
    public class I18NShopNameDAO : II18NShopNameDAO
    {
        public I18NShopNameDAO() : base()
        { }

        #region Methods

        public IEnumerable<I18NShopNameDto> FindByName(string name)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<I18NShopNameDto> result = new List<I18NShopNameDto>();
                foreach (I18NShopName item in context.I18NShopName.Where(s => string.IsNullOrEmpty(name) ? s.Text.Equals("") : s.Text.Contains(name)))
                {
                    I18NShopNameDto dto = new I18NShopNameDto();
                    Mapper.Mappers.I18NShopNameMapper.ToI18NShopNameDTO(item, dto);
                    result.Add(dto);
                }
                return result;
            }
        }
        public void Insert(List<I18NShopNameDto> skills)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    
                    foreach (I18NShopNameDto skill in skills)
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

        public SaveResult InsertOrUpdate(I18NShopNameDto skill)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long SkillVNum = skill.I18NShopNameId;
                    I18NShopName entity = context.I18NShopName.FirstOrDefault(c => c.I18NShopNameId == SkillVNum);

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
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_I18NShopName_ERROR"), skill.RegionType, e.Message), e);
                return SaveResult.Error;
            }
        }

        public I18NShopNameDto Insert(I18NShopNameDto I18NShopName)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    I18NShopName entity = new I18NShopName();
                    Mapper.Mappers.I18NShopNameMapper.ToI18NShopName(I18NShopName, entity); context.I18NShopName.Add(entity);
                    context.SaveChanges();
                    if (Mapper.Mappers.I18NShopNameMapper.ToI18NShopNameDTO(entity, I18NShopName))
                    {
                        return I18NShopName;
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

        public IEnumerable<I18NShopNameDto> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<I18NShopNameDto> result = new List<I18NShopNameDto>();
                foreach (I18NShopName I18NShopName in context.I18NShopName)
                {
                    I18NShopNameDto dto = new I18NShopNameDto();
                    Mapper.Mappers.I18NShopNameMapper.ToI18NShopNameDTO(I18NShopName, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public I18NShopNameDto LoadById(short I18NShopNameId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    I18NShopNameDto dto = new I18NShopNameDto();
                    if (Mapper.Mappers.I18NShopNameMapper.ToI18NShopNameDTO(context.I18NShopName.FirstOrDefault(s => s.I18NShopNameId.Equals(I18NShopNameId)), dto))
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

        private static I18NShopNameDto insert(I18NShopNameDto I18NShopName, OpenNosContext context)
        {
            I18NShopName entity = new I18NShopName();
            Mapper.Mappers.I18NShopNameMapper.ToI18NShopName(I18NShopName, entity);
            context.I18NShopName.Add(entity);
            context.SaveChanges();
            if (Mapper.Mappers.I18NShopNameMapper.ToI18NShopNameDTO(entity, I18NShopName))
            {
                return I18NShopName;
            }

            return null;
        }

        private static I18NShopNameDto update(I18NShopName entity, I18NShopNameDto skill, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mappers.I18NShopNameMapper.ToI18NShopName(skill, entity);
                context.SaveChanges();
            }

            if (Mapper.Mappers.I18NShopNameMapper.ToI18NShopNameDTO(entity, skill))
            {
                return skill;
            }

            return null;
        }

        #endregion
    }
}
