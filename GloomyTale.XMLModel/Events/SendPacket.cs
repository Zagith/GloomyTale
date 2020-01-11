using System;
using System.Xml.Serialization;

namespace GloomyTale.XMLModel.Events
{
    [Serializable]
    public class SendPacket
    {
        #region Properties

        [XmlAttribute]
        public string Value { get; set; }

        #endregion
    }
}