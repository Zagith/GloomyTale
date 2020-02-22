using GloomyTale.Core;
using GloomyTale.DAL.EF;
using GloomyTale.DAL.EF.Helpers;
using GloomyTale.DAL.Interface;
using GloomyTale.Data;
using GloomyTale.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace GloomyTale.DAL.DAO
{
    public class PartnerSkillDAO : MappingBaseDao<PartnerSkill, PartnerSkillDTO>, IPartnerSkillDAO
    {
        public PartnerSkillDAO(IMapper mapper) : base(mapper)
        { }

        public PartnerSkillDTO Insert(PartnerSkillDTO partnerSkillDTO)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<PartnerSkill>(partnerSkillDTO);
                    context.PartnerSkill.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<PartnerSkillDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }

            return null;
        }

        public IEnumerable<PartnerSkillDTO> LoadByEquipmentSerialId(Guid equipmentSerialId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (PartnerSkill partnerSkill in context.PartnerSkill.Where(i => i.EquipmentSerialId.Equals(equipmentSerialId)))
                {
                    yield return _mapper.Map<PartnerSkillDTO>(partnerSkill);
                }
            }
        }

        public DeleteResult Remove(long partnerSkillId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    PartnerSkill partnerSkill = context.PartnerSkill.FirstOrDefault(s => s.PartnerSkillId == partnerSkillId);

                    if (partnerSkill == null)
                    {
                        return DeleteResult.NotFound;
                    }

                    context.PartnerSkill.Remove(partnerSkill);
                    context.SaveChanges();

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }

            return DeleteResult.Error;
        }

    }
}
