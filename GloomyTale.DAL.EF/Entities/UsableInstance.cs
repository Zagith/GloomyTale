using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.DAL.EF.Entities
{
    public class UsableInstance : ItemInstance
    {
        #region Properties

        public short? HP { get; set; }

        public short? MP { get; set; }

        #endregion
    }
}
