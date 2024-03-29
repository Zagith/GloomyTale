﻿using System.ComponentModel.DataAnnotations;

namespace OpenNos.DAL.EF
{
    public class CharacterTitle
    {
        #region Properties

        [Key]
        public long CharacterTitleId { get; set; }

        public virtual Character Character { get; set; }

        public long CharacterId { get; set; }

        public bool Active { get; set; }

        public bool Visible { get; set; }

        public short TitleType { get; set; }

        #endregion
    }
}
