using AutoMapper;
using GloomyTale.DAL.Interface;
using GloomyTale.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.DAL.DAO
{
    public abstract class MappingBaseDao<TEntity, TDTO> : IMappingBaseDAO
    where TDTO : MappingBaseDTO
    {
        #region Members

        protected readonly IDictionary<Type, Type> _mappings = new Dictionary<Type, Type>();
        protected readonly IMapper _mapper;

        protected MappingBaseDao(IMapper mapper) => _mapper = mapper;

        #endregion

    }
}
