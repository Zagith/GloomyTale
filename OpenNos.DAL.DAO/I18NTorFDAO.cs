using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Entities;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data.Enums;
using OpenNos.Data.I18N;
using OpenNos.Domain.I18N;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.DAL.DAO
{
    public class I18NTorFDAO : II18NTorFDAO
    {
        #region Methods

        public IEnumerable<I18NTorFDto> FindByName(string name, RegionType regionType)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<I18NTorFDto> result = new List<I18NTorFDto>();
                foreach (I18NTorF item in context.I18NTorF.Where(s => s.RegionType == regionType && (string.IsNullOrEmpty(name) ? s.Text.Equals("") : s.Text.Contains(name))))
                {
                    I18NTorFDto dto = new I18NTorFDto();
                    Mapper.Mappers.I18NTorFMapper.ToI18NTorFDTO(item, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public void Insert(List<I18NTorFDto> skills)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    foreach (I18NTorFDto skill in skills)
                    {
                        InsertOrUpdate(skill);
                    }
                    context.Configuration.AutoDetectChangesEnabled = true;
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public SaveResult InsertOrUpdate(I18NTorFDto skill)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long SkillVNum = skill.I18NTorFId;
                    I18NTorF entity = context.I18NTorF.FirstOrDefault(c => c.I18NTorFId == SkillVNum);

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
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_I18NTorF_ERROR"), skill.RegionType, e.Message), e);
                return SaveResult.Error;
            }
        }

        public I18NTorFDto Insert(I18NTorFDto I18NTorF)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    I18NTorF entity = new I18NTorF();
                    Mapper.Mappers.I18NTorFMapper.ToI18NTorF(I18NTorF, entity); context.I18NTorF.Add(entity);
                    context.SaveChanges();
                    if (Mapper.Mappers.I18NTorFMapper.ToI18NTorFDTO(entity, I18NTorF))
                    {
                        return I18NTorF;
                    }

                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<I18NTorFDto> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<I18NTorFDto> result = new List<I18NTorFDto>();
                foreach (I18NTorF I18NTorF in context.I18NTorF)
                {
                    I18NTorFDto dto = new I18NTorFDto();
                    Mapper.Mappers.I18NTorFMapper.ToI18NTorFDTO(I18NTorF, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public I18NTorFDto LoadById(short I18NTorFId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    I18NTorFDto dto = new I18NTorFDto();
                    if (Mapper.Mappers.I18NTorFMapper.ToI18NTorFDTO(context.I18NTorF.FirstOrDefault(s => s.I18NTorFId.Equals(I18NTorFId)), dto))
                    {
                        return dto;
                    }

                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private static I18NTorFDto insert(I18NTorFDto I18NTorF, OpenNosContext context)
        {
            I18NTorF entity = new I18NTorF();
            Mapper.Mappers.I18NTorFMapper.ToI18NTorF(I18NTorF, entity);
            context.I18NTorF.Add(entity);
            context.SaveChanges();
            if (Mapper.Mappers.I18NTorFMapper.ToI18NTorFDTO(entity, I18NTorF))
            {
                return I18NTorF;
            }

            return null;
        }

        private static I18NTorFDto update(I18NTorF entity, I18NTorFDto skill, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mappers.I18NTorFMapper.ToI18NTorF(skill, entity);
                context.SaveChanges();
            }

            if (Mapper.Mappers.I18NTorFMapper.ToI18NTorFDTO(entity, skill))
            {
                return skill;
            }

            return null;
        }

        #endregion
    }
}
