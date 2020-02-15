﻿using System;
using System.Xml.Serialization;

namespace GloomyTale.XMLModel.Events
{
    [Serializable]
    public class OnAreaEntry
    {
        #region Properties

        [XmlAttribute]
        public short PositionX { get; set; }

        [XmlAttribute]
        public short PositionY { get; set; }

        [XmlAttribute]
        public byte Range { get; set; }

        [XmlElement]
        public SummonMonster[] SummonMonster { get; set; }

        #endregion
    }
}