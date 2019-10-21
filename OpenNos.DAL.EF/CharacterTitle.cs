using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.EF
{
    public class CharacterTitle
    {
        #region Properties

        [Key]
        public long CharacterTitleId { get; set; }
        public virtual Character Character { get; set; }

        public long CharacterId { get; set; }

        public long TitleId { get; set; }

        public bool IsUsed { get; set; }

        public bool IsDisplay { get; set; }

        #endregion
    }
}
