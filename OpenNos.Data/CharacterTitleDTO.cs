﻿using System;

namespace OpenNos.Data
{
    [Serializable]
    public class CharacterTitleDTO
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
