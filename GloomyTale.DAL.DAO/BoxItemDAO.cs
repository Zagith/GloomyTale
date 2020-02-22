using System.Collections.Generic;
using GloomyTale.DAL.EF;
using GloomyTale.DAL.EF.Helpers;
using GloomyTale.DAL.Interface;
using GloomyTale.Data;
using AutoMapper;

namespace GloomyTale.DAL.DAO
{
    public class BoxItemDAO : MappingBaseDao<BoxItem, BoxItemDTO>, IBoxItemDAO
    {
        public BoxItemDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public IEnumerable<BoxItemDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (BoxItem entity in context.BoxItem)
                {
                    yield return _mapper.Map<BoxItemDTO>(entity);
                }
            }
        }
        #endregion
    }
}
