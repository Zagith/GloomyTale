using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloomyTale.Data
{
    [Serializable]
    public class CharacterTitleDTO : MappingBaseDTO
    {
        #region Properties

        public long CharacterTitleId { get; set; }

        public long CharacterId { get; set; }

        public bool Active { get; set; }

        public bool Visible { get; set; }

        public short TitleType { get; set; }

        #endregion
    }
}
