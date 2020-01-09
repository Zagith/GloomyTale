﻿using OpenNos.Data;
using OpenNos.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.Interface
{
    public interface ILogPutItemDAO
    {
        LogPutItemDTO Insert(LogPutItemDTO generalLog);

        LogPutItemDTO LoadById(long id);

        IEnumerable<LogPutItemDTO> LoadByCharacterId(long id);
    }
}
