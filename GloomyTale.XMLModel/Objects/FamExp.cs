﻿using System;
using System.Xml.Serialization;

namespace GloomyTale.XMLModel.Objects
{
    [Serializable]
    public class FamExp
    {
        #region Properties

        [XmlAttribute]
        public int Value { get; set; }

        #endregion
    }
}