using Autofac;
using GloomyTale.Communication;
using GloomyTale.Communication.RPC;
using GloomyTale.Core;
using GloomyTale.DAL.EF.Helpers;
using GloomyTale.GameObject;
using GloomyTale.GameObject.Networking;
using GloomyTale.Plugins;
using GloomyTale.Plugins.Exceptions;
using GloomyTale.Plugins.Logging;
using GloomyTale.Plugins.Modules;
using GloomyTale.SqlServer;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using ConfigurationHelper = GloomyTale.World.Configuration.ConfigurationHelper;

namespace GloomyTale.World
{
    public static class Program
    {
        #region Members

        private static EventHandler _exitHandler;

        private static bool _isDebug;

        private static bool _ignoreTelemetry;

        private static short _port;

        #endregion

        #region Delegates

        public delegate bool EventHandler(CtrlType sig);

        #endregion

        #region Enums

        public enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        #endregion

        #region Methods

        private static void Welcome()
        {
            //Console.Title = string.Format(LocalizedResources.WORLD_SERVER_CONSOLE_TITLE, 0, 0, 0, 0);
        }

        private static void InitializeMasterCommunication()
        {
            int masterPort = Convert.ToInt32(Environment.GetEnvironmentVariable("MASTER_PORT") ?? "4545");
            string masterIp = Environment.GetEnvironmentVariable("MASTER_IP") ?? "127.0.0.1";
            var serviceFactory = new GRpcCommunicationServiceFactory();
            ICommunicationService service = serviceFactory.CreateService(masterIp, masterPort).ConfigureAwait(false).GetAwaiter().GetResult();
            CommunicationServiceClient.Initialize(service);
        }

        private static IContainer BuildCoreContainer()
        {
            var pluginBuilder = new ContainerBuilder();
            pluginBuilder.RegisterType<SerilogLogger>().AsImplementedInterfaces();
            // pluginBuilder.RegisterType<LoggingPlugin>().AsImplementedInterfaces().AsSelf();
            pluginBuilder.RegisterType<DatabasePlugin>().AsImplementedInterfaces().AsSelf();
            IContainer container = pluginBuilder.Build();

            var coreBuilder = new ContainerBuilder();
            foreach (ICorePlugin plugin in container.Resolve<IEnumerable<ICorePlugin>>())
            {
                try
                {
                    plugin.OnLoad(coreBuilder);
                }
                catch (PluginException e)
                {
                }
            }

            coreBuilder.RegisterType<WorldServiceImpl>();
            //coreBuilder.Register(_ => new GameObjectMapper()).As<IMapper>().SingleInstance();
            return coreBuilder.Build();
        }

        /*private static void CustomisationRegistration()
        {
            const string configPath = "./config/";
            DependencyContainer.Instance.Register(ConfigurationHelper.Load<BaseCharacter>(configPath + nameof(BaseCharacter) + ".json", true));
            Logger.Log.Info("[CUSTOMIZER] BaseCharacter Loaded !");
            DependencyContainer.Instance.Register(ConfigurationHelper.Load<BaseQuicklist>(configPath + nameof(BaseQuicklist) + ".json", true));
            Logger.Log.Info("[CUSTOMIZER] BaseQuicklist Loaded !");
            DependencyContainer.Instance.Register(ConfigurationHelper.Load<BaseInventory>(configPath + nameof(BaseInventory) + ".json", true));
            Logger.Log.Info("[CUSTOMIZER] BaseInventory Loaded !");
            DependencyContainer.Instance.Register(ConfigurationHelper.Load<BaseSkill>(configPath + nameof(BaseSkill) + ".json", true));
            Logger.Log.Info("[CUSTOMIZER] BaseSkill Loaded !");

            DependencyContainer.Instance.Register(ConfigurationHelper.Load<GameRateConfiguration>(configPath + "game.json", true));
            Logger.Log.Info("[CUSTOMIZER] Game Rate            Configuration Loaded !");
            DependencyContainer.Instance.Register(ConfigurationHelper.Load<GameMinMaxConfiguration>(configPath + "min_max.json", true));
            Logger.Log.Info("[CUSTOMIZER] Game MinMax          Configuration Loaded !");
            DependencyContainer.Instance.Register(ConfigurationHelper.Load<GameTrueFalseConfiguration>(configPath + "events.json", true));
            Logger.Log.Info("[CUSTOMIZER] Game TrueFalse       Configuration Loaded !");
            DependencyContainer.Instance.Register(ConfigurationHelper.Load<GameScheduledEventsConfiguration>(configPath + "scheduled_events.json", true));
            Logger.Log.Info("[CUSTOMIZER] Game ScheduledEvents Configuration Loaded !");
        }*/

        public static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            Welcome();

            bool ignoreStartupMessages = false;
            _port = 3438;
            int portArgIndex = Array.FindIndex(args, s => s == "--port");
            if (portArgIndex != -1
                && args.Length >= portArgIndex + 1
                && short.TryParse(args[portArgIndex + 1], out _port))
            {
                Console.WriteLine("Port override: " + _port);
            }
            foreach (string arg in args)
            {
                switch (arg)
                {
                    case "--nomsg":
                        ignoreStartupMessages = true;
                        break;

                    case "--notelemetry":
                        _ignoreTelemetry = true;
                        break;
                }
            }

            if (!ignoreStartupMessages)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                string text = $"WORLD SERVER v{fileVersionInfo.ProductVersion}dev - PORT : {_port} by OpenNos Team";
                int offset = (Console.WindowWidth / 2) + (text.Length / 2);
                string separator = new string('=', Console.WindowWidth);
                Console.WriteLine(separator + string.Format("{0," + offset + "}\n", text) + separator);
            }

            Logger.InitializeLogger(new SerilogLogger());

            using (IContainer coreContainer = BuildCoreContainer())
            {
                var gameBuilder = new ContainerBuilder();
                gameBuilder.RegisterInstance(coreContainer).As<IContainer>();
                gameBuilder.RegisterModule(new CoreContainerModule(coreContainer));
                IContainer gameContainer = gameBuilder.Build();
                IEnumerable<IGamePlugin> plugins = gameContainer.Resolve<IEnumerable<IGamePlugin>>();
                if (plugins != null)
                {
                    foreach (IGamePlugin gamePlugin in plugins)
                    {
                        gamePlugin.OnEnable();
                        gamePlugin.OnDisable();
                    }
                }

                // initialize Loggers
                //CustomisationRegistration();

                int gRpcPort = Convert.ToInt32(Environment.GetEnvironmentVariable("GRPC_PORT") ?? "17500");
                string gRpcIp = Environment.GetEnvironmentVariable("GRPC_IP") ?? "localhost";
                var gRpcEndPoint = new GRpcEndPoint
                {
                    Ip = gRpcIp,
                    Port = gRpcPort
                };
                var gRpcServer = new Server
                {
                    Services = { global::World.BindService(coreContainer.Resolve<WorldServiceImpl>()) },
                    Ports = { new ServerPort(gRpcEndPoint.Ip, gRpcEndPoint.Port, ServerCredentials.Insecure) }
                };
                Logger.Log.Info($"[RPC-Server] Listening on {gRpcEndPoint.Ip}:{gRpcEndPoint.Port}");
                gRpcServer.Start();

                InitializeMasterCommunication();

                // initialize api
                if (CommunicationServiceClient.Instance.IsMasterOnline())
                {
                    Logger.Log.Info(Language.Instance.GetMessageFromKey("API_INITIALIZED"));
                }

                // initialize DB
                if (!DataAccessHelper.Initialize(coreContainer.Resolve<IOpenNosContextFactory>()))
                {
                    Console.ReadKey();
                    return;
                }

                //DaoFactory.Initialize(coreContainer.Resolve<DaoFactory>());

                PacketFactory.Initialize<WalkPacket>();
                string ip = "127.0.0.1";


                WorldServer server;
            portloop:
                try
                {
                    _exitHandler += ExitHandler;
                    AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
                    NativeMethods.SetConsoleCtrlHandler(_exitHandler, true);

                    server = new WorldServer(IPAddress.Any, _port);
                    server.Start();
                }
                catch (SocketException ex)
                {
                    if (ex.ErrorCode == 10048)
                    {
                        _port++;
                        Logger.Log.Info("Port already in use! Incrementing...");
                        goto portloop;
                    }

                    Logger.Log.Error("General Error", ex);
                    Environment.Exit(1);
                    return;
                }

                ServerManager.Instance.ServerGroup = Environment.GetEnvironmentVariable("SERVER_GROUP") ?? "WingsEmu";
                int sessionLimit = Convert.ToInt32(Environment.GetEnvironmentVariable("SERVER_SESSION_LIMIT") ?? "500");
                int? newChannelId = CommunicationServiceClient.Instance.RegisterWorldServer(new SerializableWorldServer
                {
                    Id = ServerManager.Instance.WorldId,
                    EndPointIp = ip,
                    EndPointPort = _port,
                    AccountLimit = sessionLimit,
                    WorldGroup = ServerManager.Instance.ServerGroup
                }, gRpcEndPoint);

                if (newChannelId.HasValue)
                {
                    ServerManager.Instance.ChannelId = newChannelId.Value;
                    ServerManager.Instance.IpAddress = ip;
                    ServerManager.Instance.Port = _port;
#warning TODO Session limit
                    //ServerManager.Instance.AccountLimit = sessionLimit;
                    Console.Title = string.Format(Language.Instance.GetMessageFromKey("WORLD_SERVER_CONSOLE_TITLE"), ServerManager.Instance.ChannelId, ServerManager.Instance.Sessions.Count(),
                        ServerManager.Instance.IpAddress, ServerManager.Instance.Port);
                }
                else
                {
                    server.Stop();
                    Logger.Log.Error("Could not retrieve ChannelId from Web API.", null);
                    Console.ReadKey();
                }

                while (!ServerManager.Instance.InShutdown)
                {
                    string tmp = Console.ReadLine();
                    if (tmp == "quit")
                    {
                        break;
                    }
                }
                server.Stop();
                gRpcServer.ShutdownAsync().ConfigureAwait(false).GetAwaiter().GetResult();

#if !DEBUG
                DiscordHelper serverStatus = new DiscordHelper();
#endif
            }
        }

        private static bool ExitHandler(CtrlType sig)
        {
            CommunicationServiceClient.Instance.UnregisterWorldServer(ServerManager.Instance.WorldId);
            ServerManager.Shout(string.Format(Language.Instance.GetMessageFromKey("SHUTDOWN_SEC"), 5));
            foreach (ClientSession sess in ServerManager.Instance.Sessions)
            {
                sess.Character?.Dispose();
            }
            ServerManager.Instance.SaveAll();
            Thread.Sleep(5000);
            return false;
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            ServerManager.Instance.InShutdown = true;
            Logger.Error((Exception)e.ExceptionObject);

            File.AppendAllText("C:\\WORLD_CRASHLOG.txt", e.ExceptionObject.ToString() + "\n");

            Logger.Log.Debug("Server crashed! Rebooting gracefully...");
            CommunicationServiceClient.Instance.UnregisterWorldServer(ServerManager.Instance.WorldId);
            ServerManager.Shout(string.Format(Language.Instance.GetMessageFromKey("SHUTDOWN_SEC"), 5));
            ServerManager.Instance.SaveAll();
            foreach (ClientSession sess in ServerManager.Instance.Sessions)
            {
                sess.Character?.Dispose();
            }
            Process.Start("GloomyTale.World.exe", $"--nomsg --port {_port}");
            Environment.Exit(1);
        }

        #endregion

        #region Classes
        public static class NativeMethods
        {
            [DllImport("Kernel32")]
            internal static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add); 
        }
        #endregion
    }
}
