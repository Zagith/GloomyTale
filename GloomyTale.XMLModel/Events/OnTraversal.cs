﻿using System;
using System.Xml.Serialization;

namespace GloomyTale.XMLModel.Events
{
    [Serializable]
    public class OnTraversal
    {
        #region Properties

        [XmlElement]
        public End End { get; set; }

        [XmlElement]
        public NpcDialog[] NpcDialog { get; set; }

        #endregion
    }
}