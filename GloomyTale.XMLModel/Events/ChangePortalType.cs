using System;
using System.Xml.Serialization;

namespace GloomyTale.XMLModel.Events
{
    [Serializable]
    public class ChangePortalType
    {
        #region Properties

        [XmlAttribute]
        public int IdOnMap { get; set; }

        [XmlAttribute]
        public sbyte Type { get; set; }
        
        [XmlAttribute]
        public int Map { get; set; }

        #endregion
    }
}