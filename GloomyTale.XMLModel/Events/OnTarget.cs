using System;
using System.Xml.Serialization;

namespace GloomyTale.XMLModel.Events
{
    [Serializable]
    public class OnTarget
    {
        #region Properties

        [XmlElement]
        public Effect Effect { get; set; }

        [XmlElement]
        public Move Move { get; set; }

        [XmlElement]
        public ControlMonsterInRange ControlMonsterInRange { get; set; }

        #endregion
    }
}