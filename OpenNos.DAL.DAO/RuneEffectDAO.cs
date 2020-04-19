using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using OpenNos.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.DAL.DAO
{
    public class RuneEffectDAO : IRuneEffectDAO
    {
        #region Methods

        public DeleteResult DeleteByEquipmentSerialId(Guid id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    List<RuneEffect> deleteentities = context.RuneEffect.Where(s => s.EquipmentSerialId == id).ToList();
                    if (deleteentities.Count != 0)
                    {
                        context.RuneEffect.RemoveRange(deleteentities);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_ERROR"), id, e.Message), e);
                return DeleteResult.Error;
            }
        }

        public RuneEffectDTO InsertOrUpdate(RuneEffectDTO runeEffect)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long runeEffectId = runeEffect.RuneEffectId;
                    RuneEffect entity = context.RuneEffect.FirstOrDefault(c => c.RuneEffectId.Equals(runeEffectId));

                    if (entity == null)
                    {
                        return Insert(runeEffect, context);
                    }
                    return Update(entity, runeEffect, context);
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("INSERT_ERROR"), runeEffect, e.Message), e);
                return runeEffect;
            }
        }

        public void InsertOrUpdateFromList(List<RuneEffectDTO> runeEffect, Guid equipmentSerialId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    void Insert(RuneEffectDTO runeeffect)
                    {
                        RuneEffect entity = new RuneEffect();
                        Mapper.Mappers.RuneEffectMapper.ToRuneEffect(runeeffect, entity);
                        context.RuneEffect.Add(entity);
                        context.SaveChanges();
                        runeeffect.RuneEffectId = entity.RuneEffectId;
                    }

                    void Update(RuneEffect entity, RuneEffectDTO runeeffect)
                    {
                        if (entity != null)
                        {
                            Mapper.Mappers.RuneEffectMapper.ToRuneEffect(runeeffect, entity);
                        }
                    }

                    foreach (RuneEffectDTO item in runeEffect)
                    {
                        item.EquipmentSerialId = equipmentSerialId;
                        RuneEffect entity = context.RuneEffect.FirstOrDefault(c => c.RuneEffectId == item.RuneEffectId);

                        if (entity == null)
                        {
                            Insert(item);
                        }
                        else
                        {
                            Update(entity, item);
                        }
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public IEnumerable<RuneEffectDTO> LoadByEquipmentSerialId(Guid id)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<RuneEffectDTO> result = new List<RuneEffectDTO>();
                foreach (RuneEffect entity in context.RuneEffect.AsNoTracking().Where(c => c.EquipmentSerialId == id))
                {
                    RuneEffectDTO dto = new RuneEffectDTO();
                    Mapper.Mappers.RuneEffectMapper.ToRuneEffectDTO(entity, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        private static RuneEffectDTO Insert(RuneEffectDTO runeeffect, OpenNosContext context)
        {
            RuneEffect entity = new RuneEffect();
            Mapper.Mappers.RuneEffectMapper.ToRuneEffect(runeeffect, entity);
            context.RuneEffect.Add(entity);
            context.SaveChanges();
            if (Mapper.Mappers.RuneEffectMapper.ToRuneEffectDTO(entity, runeeffect))
            {
                return runeeffect;
            }

            return null;
        }

        private static RuneEffectDTO Update(RuneEffect entity, RuneEffectDTO runeeffect, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mappers.RuneEffectMapper.ToRuneEffect(runeeffect, entity);
                context.SaveChanges();
            }

            if (Mapper.Mappers.RuneEffectMapper.ToRuneEffectDTO(entity, runeeffect))
            {
                return runeeffect;
            }

            return null;
        }

        #endregion
    }
}