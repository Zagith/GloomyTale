using AutoMapper;
using EFCore.BulkExtensions;
using GloomyTale.Core;
using GloomyTale.DAL.EF;
using GloomyTale.DAL.EF.Helpers;
using GloomyTale.DAL.Interface;
using GloomyTale.Data;
using GloomyTale.Data.Enums;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GloomyTale.DAL.DAO
{
    public abstract class SynchronizableBaseDAO<TEntity, TDTO> : MappingBaseDao<TEntity, TDTO>, ISynchronizableBaseDAO<TDTO>
    where TDTO : SynchronizableBaseDTO
    where TEntity : SynchronizableBaseEntity
    {
        protected SynchronizableBaseDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public virtual DeleteResult Delete(IEnumerable<Guid> ids)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Guid id in ids)
                {
                    TEntity entity = context.Set<TEntity>().FirstOrDefault(i => i.Id == id);
                    if (entity != null)
                    {
                        context.Set<TEntity>().Remove(entity);
                    }
                }

                context.SaveChanges();
                return DeleteResult.Deleted;
            }
        }

        public virtual DeleteResult Delete(Guid id)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                TEntity entity = context.Set<TEntity>().FirstOrDefault(i => i.Id == id);
                if (entity == null)
                {
                    return DeleteResult.Deleted;
                }

                context.Set<TEntity>().Remove(entity);
                context.SaveChanges();

                return DeleteResult.Deleted;
            }
        }

        public IEnumerable<TDTO> InsertOrUpdate(IEnumerable<TDTO> dtos)
        {
            try
            {
                IList<TDTO> results = new List<TDTO>();
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (TDTO dto in dtos)
                    {
                        results.Add(InsertOrUpdate(context, dto));
                    }
                }

                return results;
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_ERROR"), e.Message), e);
                return Enumerable.Empty<TDTO>();
            }
        }

        public TDTO InsertOrUpdate(TDTO dto)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return InsertOrUpdate(context, dto);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_ERROR"), e.Message), e);
                return null;
            }
        }

        public TDTO LoadById(Guid id)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return _mapper.Map<TDTO>(context.Set<TEntity>().FirstOrDefault(i => i.Id.Equals(id)));
            }
        }

        protected virtual TDTO Insert(TDTO dto, OpenNosContext context)
        {
            TEntity entity = MapEntity(dto);
            context.Set<TEntity>().Add(entity);
            context.SaveChanges();
            return _mapper.Map<TDTO>(entity);
        }

        protected virtual TDTO InsertOrUpdate(OpenNosContext context, TDTO dto)
        {
            Guid primaryKey = dto.Id;
            TEntity entity = context.Set<TEntity>().FirstOrDefault(c => c.Id == primaryKey);
            dto = entity == null ? Insert(dto, context) : Update(entity, dto, context);

            return dto;
        }

        protected virtual TEntity MapEntity(TDTO dto) => _mapper.Map<TEntity>(dto);

        protected virtual TDTO Update(TEntity entity, TDTO inventory, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(inventory, entity);
            }

            return _mapper.Map<TDTO>(entity);
        }

        public virtual TDTO Save(TDTO obj)
        {
            try
            {
                
                using (OpenNosContext Context = DataAccessHelper.CreateContext())
                {
                    TEntity model = Context.Set<TEntity>().Find(obj.Id);
                    if (model == null)
                    {
                        if (obj.Id == Guid.Empty)
                        {
                            obj.Id = Guid.NewGuid();
                        }

                        model = Context.Set<TEntity>().Add(_mapper.Map<TEntity>(obj)).Entity;
                    }
                    else
                    {
                        Context.Entry(model).CurrentValues.SetValues(obj);
                    }

                    Context.SaveChanges();
                    return _mapper.Map<TDTO>(model);
                }                
            }
            catch (Exception e)
            {
                Log.Error("[SAVE]", e);
                return null;
            }
        }

        public virtual void Save(IEnumerable<TDTO> objs)
        {
            try
            {
                IEnumerable<TDTO> enumerable = objs as TDTO[] ?? objs.ToArray();
                if (enumerable.All(s => s == null))
                {
                    return;
                }


                List<TEntity> tmp = enumerable.Where(s => s != null).Select(_mapper.Map<TEntity>).ToList();
                using (OpenNosContext Context = DataAccessHelper.CreateContext())
                {
                    using (IDbContextTransaction transaction = Context.Database.BeginTransaction())
                    {
                        Context.BulkInsertOrUpdate(tmp, new BulkConfig
                        {
                            PreserveInsertOrder = true
                        });
                        transaction.Commit();
                        Context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("[SAVE]", e);
            }
        }
        #endregion
    }
}
