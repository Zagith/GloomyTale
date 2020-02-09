using System;

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
