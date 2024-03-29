﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Master.Server.Controllers.ControllersParameters
{
    public class MailPostParameter
    {
        public long CharacterId { get; set; }
        public string WorldGroup { get; set; }
        public string Title { get; set; }
        public short VNum { get; set; }
        public short Amount { get; set; }
        public sbyte Rare { get; set; }
        public byte Upgrade { get; set; }
        public bool IsNosmall { get; set; }
        public bool IsAchievement { get; set; }
        public long? AchievementId { get; set; }
    }
}
