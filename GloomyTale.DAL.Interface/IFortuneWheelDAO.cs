using OpenNos.Data;
using System.Collections.Generic;

namespace OpenNos.DAL.Interface
{
    public interface IFortuneWheelDAO
    {
        IEnumerable<FortuneWheelDTO> LoadByShopId(int shopId);
    }
}
