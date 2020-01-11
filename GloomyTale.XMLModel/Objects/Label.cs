﻿using System;
using System.Xml.Serialization;

namespace GloomyTale.XMLModel.Objects
{
    [Serializable]
    public class Label
    {
        #region Properties

        [XmlAttribute]
        public string Value { get; set; }

        #endregion
    }
}