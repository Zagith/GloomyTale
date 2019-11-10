using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Entities;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data.Enums;
using OpenNos.Data.I18N;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.DAO
{
    public class I18NItemDAO : II18NItemDAO
    {
        #region Methods

        public void Insert(List<I18NItemDto> skills)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    foreach (I18NItemDto skill in skills)
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
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_I18NITEM_ERROR"), skill.RegionType, e.Message), e);
                return SaveResult.Error;
            }
        }

        public I18NItemDto Insert(I18NItemDto i18NItem)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    I18NItem entity = new I18NItem();
                    Mapper.Mappers.I18NItemMapper.ToI18NItem(i18NItem, entity); context.I18NItem.Add(entity);
                    context.SaveChanges();
                    if (Mapper.Mappers.I18NItemMapper.ToI18NItemDTO(entity, i18NItem))
                    {
                        return i18NItem;
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

        public IEnumerable<I18NItemDto> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<I18NItemDto> result = new List<I18NItemDto>();
                foreach (I18NItem I18NItem in context.I18NItem)
                {
                    I18NItemDto dto = new I18NItemDto();
                    Mapper.Mappers.I18NItemMapper.ToI18NItemDTO(I18NItem, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public I18NItemDto LoadById(short i18NItemId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    I18NItemDto dto = new I18NItemDto();
                    if (Mapper.Mappers.I18NItemMapper.ToI18NItemDTO(context.I18NItem.FirstOrDefault(s => s.I18NItemId.Equals(i18NItemId)), dto))
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

        private static I18NItemDto insert(I18NItemDto i18NItem, OpenNosContext context)
        {
            I18NItem entity = new I18NItem();
            Mapper.Mappers.I18NItemMapper.ToI18NItem(i18NItem, entity);
            context.I18NItem.Add(entity);
            context.SaveChanges();
            if (Mapper.Mappers.I18NItemMapper.ToI18NItemDTO(entity, i18NItem))
            {
                return i18NItem;
            }

            return null;
        }

        private static I18NItemDto update(I18NItem entity, I18NItemDto skill, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mappers.I18NItemMapper.ToI18NItem(skill, entity);
                context.SaveChanges();
            }

            if (Mapper.Mappers.I18NItemMapper.ToI18NItemDTO(entity, skill))
            {
                return skill;
            }

            return null;
        }

        #endregion
    }
}
