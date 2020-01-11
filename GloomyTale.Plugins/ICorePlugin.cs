using Autofac;

namespace GloomyTale.Plugins
{
    public interface ICorePlugin : IPlugin
    {
        /// <summary>
        ///     Loads the plugin with the given container builder to register dependencies
        /// </summary>
        /// <param name="builder"></param>
        void OnLoad(ContainerBuilder builder);
    }
}
