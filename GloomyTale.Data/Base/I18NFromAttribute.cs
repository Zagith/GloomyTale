using System;

namespace OpenNos.Data.Base
{
    public class I18NFromAttribute : Attribute
    {
        public I18NFromAttribute(Type type)
        {
            Type = type;
        }

        public Type Type { get; set; }
    }
}
