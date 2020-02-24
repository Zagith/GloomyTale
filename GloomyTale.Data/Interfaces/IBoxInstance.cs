using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.Data.Interfaces
{
    public interface IBoxInstance : ISpecialistInstance
    {
        #region Properties

        short HoldingVNum { get; set; }

        #endregion
    }
}
