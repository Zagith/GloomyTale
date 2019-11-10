using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.EF.Base
{
    public class I18NStringAttribute : Attribute
    {
        public I18NStringAttribute(Type type)
        {
            Type = type;
        }

        public Type Type { get; set; }
    }
}
