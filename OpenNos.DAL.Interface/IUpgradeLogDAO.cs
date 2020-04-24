﻿using OpenNos.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.Interface
{
    public interface IUpgradeLogDAO
    {
        UpgradeLogDTO Insert(UpgradeLogDTO generalLog);

        IEnumerable<UpgradeLogDTO> LoadByCharacterId(long id);
    }
}
