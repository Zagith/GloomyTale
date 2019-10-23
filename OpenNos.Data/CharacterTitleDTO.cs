using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Data
{
    [Serializable]
    public class CharacterTitleDTO
    {
        #region Properties

        public long CharacterTitleId { get; set; }

        public long CharacterId { get; set; }

        public long TitleId { get; set; }

        public bool IsUsed { get; set; }

        public bool IsDisplay { get; set; }

        #endregion
    }
}
