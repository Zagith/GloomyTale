using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.Data.Interfaces
{
    public interface IItemInstance
    {
        #region Properties

        short Amount { get; set; }

        long? BoundCharacterId { get; set; }

        short Design { get; set; }

        Guid Id { get; set; }

        DateTime? ItemDeleteTime { get; set; }

        short ItemVNum { get; set; }

        sbyte Rare { get; set; }

        byte Upgrade { get; set; }

        #endregion
    }
}
