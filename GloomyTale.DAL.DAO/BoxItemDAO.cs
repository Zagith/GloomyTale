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

        public List<BoxItemDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<BoxItemDTO> result = new List<BoxItemDTO>();

                foreach (BoxItem boxItem in context.BoxItem)
                {
                    BoxItemDTO dto = new BoxItemDTO();
                    Mapper.Mappers.BoxItemMapper.ToBoxItemDTO(boxItem, dto);
                    result.Add(dto);
                }

                return result;
            }
        }

        #endregion
    }
}
