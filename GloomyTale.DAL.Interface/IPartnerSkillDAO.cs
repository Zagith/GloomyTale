using GloomyTale.Data;
using GloomyTale.Data.Enums;
using System;
using System.Collections.Generic;

namespace GloomyTale.DAL.Interface
{
    public interface IPartnerSkillDAO : IMappingBaseDAO
    {
        PartnerSkillDTO Insert(PartnerSkillDTO partnerSkillDTO);

        IEnumerable<PartnerSkillDTO> LoadByEquipmentSerialId(Guid equipmentSerialId);

        DeleteResult Remove(long partnerSkillId);
    }
}
