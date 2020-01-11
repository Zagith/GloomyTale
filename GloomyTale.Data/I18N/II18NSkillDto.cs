﻿using GloomyTale.Data.Interfaces;
using GloomyTale.Domain.I18N;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloomyTale.Data.I18N
{
    public class II18NSkillDto : II18NDto
    {
        public int I18NSkillId { get; set; }

        public string Key { get; set; }
        public RegionType RegionType { get; set; }
        public string Text { get; set; }
    }
}
