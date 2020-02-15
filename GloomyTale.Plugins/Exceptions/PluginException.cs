using System;

namespace GloomyTale.Plugins.Exceptions
{
    public class PluginException : Exception
    {
        public PluginException(string message) : base(message)
        {
        }
    }
}
