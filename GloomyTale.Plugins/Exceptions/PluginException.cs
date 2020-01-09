using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.Plugins.Exceptions
{
    public class PluginException : Exception
    {
        public PluginException(string message) : base(message)
        {
        }
    }
}
