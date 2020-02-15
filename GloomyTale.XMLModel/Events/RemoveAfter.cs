﻿using System;
using System.Xml.Serialization;

namespace GloomyTale.XMLModel.Events
{
    [Serializable]
    public class RemoveAfter
    {
        #region Properties

        [XmlAttribute]
        public short Value { get; set; }

        #endregion
    }
}
