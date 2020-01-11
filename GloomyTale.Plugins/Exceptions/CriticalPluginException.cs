namespace GloomyTale.Plugins.Exceptions
{
    public class CriticalPluginException : PluginException
    {
        public CriticalPluginException(IPlugin plugin, string message = "Critical Plugin Exception") : base($"[{plugin.Name}] {message}") => Plugin = plugin;

        public IPlugin Plugin { get; }
    }
}
