using Autofac;
using GloomyTale.Core.Extensions;
using GloomyTale.DAL.EF;
using GloomyTale.DAL.EF.Base;
using GloomyTale.Plugins;
using GloomyTale.Plugins.Logging.Interface;
using GloomyTale.DAL;
using GloomyTale.DAL.DAO;
using GloomyTale.Data;
using GloomyTale.DAL.Interface;

namespace GloomyTale.SqlServer
{
    public class DatabasePlugin : ICorePlugin
    {
        private readonly ILogger _log;

        public DatabasePlugin(ILogger log)
        {
            _log = log;
        }

        public string Name => nameof(DatabasePlugin);

        public void OnDisable()
        {
        }

        public void OnEnable()
        {
        }

        public void OnLoad(ContainerBuilder builder)
        {
            _log.Info("Registering DAL.EF objects");
            builder.RegisterTypes(typeof(AccountDAO).Assembly.GetTypesImplementingInterface <IMappingBaseDAO> ()).AsImplementedInterfaces().AsSelf();
            builder.RegisterType<DbContextFactory>().AsImplementedInterfaces().AsSelf();
            builder.RegisterType<DatabaseConfiguration>().AsImplementedInterfaces().AsSelf();
            _log.Info("Registering DAL objects");
            builder.RegisterType(typeof(DAOFactory)).AsSelf();
            _log.Info("Registering Mapping objects");
            //builder.Register(_ => new GloomyItemInstanceMappingType()).As<ItemInstanceDAO.IItemInstanceMappingTypes>();
            _log.Info("Registering DAL.EF.DAO objects");
            builder.RegisterTypes(typeof(OpenNosContext).Assembly.GetTypes()).AsSelf().AsImplementedInterfaces().SingleInstance();
        }
    }
}
