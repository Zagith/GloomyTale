using OpenNos.Data.Achievements;
using OpenNos.Domain;
using System.Collections.Generic;

namespace OpenNos.DAL.Interface
{
    public interface IAchievementsDAO
    {
        AchievementsDTO LoadById(long accountId);

        AchievementsDTO LoadByData2(int accountId);

        IEnumerable<AchievementsDTO> LoadByType(int type);

        AchievementsDTO LoadByTypeAndData2(int type, int index);
    }
}
