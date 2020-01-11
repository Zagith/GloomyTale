using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GloomyTale.Plugins
{
    public interface IPluginManager
    {
        IPlugin[] LoadPlugin(FileInfo file);
        IPlugin[] LoadPlugins(DirectoryInfo directory);
    }
}
