using System.IO;

namespace GloomyTale.Plugins
{
    public interface IPluginManager
    {
        IPlugin[] LoadPlugin(FileInfo file);
        IPlugin[] LoadPlugins(DirectoryInfo directory);
    }
}
