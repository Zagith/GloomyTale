using Autofac;
using GloomyTale.Plugins.Logging.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.Plugins.Logging
{
    public class LoggingPlugin : ICorePlugin
    {
        public string Name => nameof(LoggingPlugin);

        public void OnDisable()
        {
        }

        public void OnEnable()
        {
        }

        public void OnLoad(ContainerBuilder builder)
        {
            builder.RegisterType<SerilogLogger>().As<ILogger>();
        }
    }
}
