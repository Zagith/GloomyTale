using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenNos.XMLModel.Events
{
    [Serializable]
    public class OnStatueOpen
    {
        #region Properties

        [XmlElement]
        public IncreaseCounter SetStatueLockers { get; set; }

        [XmlElement]
        public OnStatueOpen RefreshOnStatueLockerOpen { get; set; }

        [XmlElement]
        public SummonMonster[] SummonMonster { get; set; }

        #endregion
    }
}
