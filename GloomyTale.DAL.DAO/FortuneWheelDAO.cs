using GloomyTale.DAL.EF;
using GloomyTale.DAL.EF.Entities;
using GloomyTale.DAL.EF.Helpers;
using GloomyTale.DAL.Interface;
using GloomyTale.Data;
using System.Collections.Generic;
using System.Linq;

namespace GloomyTale.DAL.DAO
{
    public class FortuneWheelDAO : IFortuneWheelDAO
    {
        public FortuneWheelDAO() : base()
        { }

        public IEnumerable<FortuneWheelDTO> LoadByShopId(int shopId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<FortuneWheelDTO> result = new List<FortuneWheelDTO>();

                foreach (FortuneWheel FortuneWheel in context.FortuneWheel.Where(s => s.ShopId == shopId))
                {
                    FortuneWheelDTO dto = new FortuneWheelDTO();
                    Mapper.Mappers.FortuneWheelMapper.ToFortuneWheelDTO(FortuneWheel, dto);
                    result.Add(dto);
                }
                return result;
            }
        }
    }
}
