using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.Plugins
{
    public interface IGamePlugin : IPlugin
    {
        /// <summary>
        ///     Called when this plugin is loaded but before it has been enabled
        /// </summary>
        void OnLoad();
    }
}
