using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenNos.XMLModel.Events
{
    [Serializable]
    public class IncreaseCounter
    {
        #region Properties

        [XmlAttribute]
        public byte Value { get; set; }

        #endregion
    }
}
