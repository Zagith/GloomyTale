using System;
using System.Xml.Serialization;

namespace GloomyTale.XMLModel.Events
{
    [Serializable]
    public class Effect
    {
        #region Properties

        [XmlAttribute]
        public short Value { get; set; }

        [XmlAttribute]
        public int Delay { get; set; }

        #endregion
    }
}