using GloomyTale.Data;
using System.Collections.Generic;

namespace GloomyTale.DAL.Interface
{
    public interface IFortuneWheelDAO
    {
        IEnumerable<FortuneWheelDTO> LoadByShopId(int shopId);
    }
}
