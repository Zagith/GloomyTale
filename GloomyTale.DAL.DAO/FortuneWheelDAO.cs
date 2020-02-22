using GloomyTale.DAL.EF;
using GloomyTale.DAL.EF.Entities;
using GloomyTale.DAL.EF.Helpers;
using GloomyTale.DAL.Interface;
using GloomyTale.Data;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace GloomyTale.DAL.DAO
{
    public class FortuneWheelDAO : MappingBaseDao<FortuneWheel, FortuneWheelDTO>, IFortuneWheelDAO
    {
        public FortuneWheelDAO(IMapper mapper) : base(mapper)
        { }

        public IEnumerable<FortuneWheelDTO> LoadByShopId(int shopId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (FortuneWheel ShopItem in context.FortuneWheel.Where(i => i.ShopId.Equals(shopId)))
                {
                    yield return _mapper.Map<FortuneWheelDTO>(ShopItem);
                }
            }
        }
    }
}
