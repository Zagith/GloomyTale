using OpenNos.Data.Enums;
using OpenNos.Data.I18N;
using OpenNos.Domain.I18N;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.Interface
{
    public interface II18NTorFDAO
    {
        #region Methods

        IEnumerable<I18NTorFDto> FindByName(string name, RegionType regionType);

        I18NTorFDto Insert(I18NTorFDto teleporter);

        void Insert(List<I18NTorFDto> skills);

        SaveResult InsertOrUpdate(I18NTorFDto teleporter);

        IEnumerable<I18NTorFDto> LoadAll();

        I18NTorFDto LoadById(short teleporterId);

        #endregion
    }
}
