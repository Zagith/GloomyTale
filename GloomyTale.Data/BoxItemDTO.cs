using GloomyTale.Data.Interfaces;

namespace GloomyTale.Data
{
    public class BoxItemDTO : SpecialistInstanceDTO, IBoxInstance
    {
        #region Properties

        public short HoldingVNum { get; set; }

        #endregion
    }
}