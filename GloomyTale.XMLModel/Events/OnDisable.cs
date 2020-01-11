﻿using System;
using System.Xml.Serialization;

namespace GloomyTale.XMLModel.Events
{
    [Serializable]
    public class OnDisable
    {
        #region Properties

        [XmlElement]
        public Teleport Teleport { get; set; }

        [XmlElement]
        public NpcDialog[] NpcDialog { get; set; }

        #endregion
    }
}