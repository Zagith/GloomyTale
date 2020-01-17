using GloomyTale.Data;
using System.Collections.Generic;

namespace GloomyTale.DAL.Interface
{
    public interface IFortuneWheelDAO : IMappingBaseDAO
    {
        IEnumerable<FortuneWheelDTO> LoadByShopId(int shopId);
    }
}
