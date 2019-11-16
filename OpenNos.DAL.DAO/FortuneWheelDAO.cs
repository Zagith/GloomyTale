using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.DAL.DAO
{
    public class FortuneWheelDAO : IFortuneWheelDAO
    {
        public IEnumerable<FortuneWheelDTO> LoadByShopId(int shopId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<FortuneWheelDTO> result = new List<FortuneWheelDTO>();

                /*foreach (FortuneWheel FortuneWheel in context.FortuneWheel.Where(s => s.ShopId == shopId))
                {
                    FortuneWheelDTO dto = new FortuneWheelDTO();
                    Mapper.Mappers.FortuneWheelMapper.ToFortuneWheelDTO(FortuneWheel, dto);
                    result.Add(dto);
                }*/
                return result;
            }
        }
    }
}
