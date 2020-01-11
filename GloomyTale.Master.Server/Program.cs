/*
 * This file is part of the OpenNos Emulator Project. See AUTHORS file for Copyright information
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 */

using Autofac;
using GloomyTale.Communication.RPC;
using GloomyTale.Master;
using GloomyTale.Plugins;
using GloomyTale.Plugins.Exceptions;
using GloomyTale.Plugins.Modules;
using GloomyTale.SqlServer;
using Grpc.Core;
using GloomyTale.Core;
using GloomyTale.DAL.EF.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using GloomyTale.Plugins.Logging;
using ILogger = GloomyTale.Plugins.Logging.Interface.ILogger;

namespace GloomyTale.Master
{
    internal static class Program
    {
        #region Methods
        private static IContainer InitializePlugins()
        {
            var pluginBuilder = new ContainerBuilder();
            pluginBuilder.RegisterType<SerilogLogger>().AsImplementedInterfaces().AsSelf();
            pluginBuilder.RegisterType<LoggingPlugin>().AsImplementedInterfaces().AsSelf();
            pluginBuilder.RegisterType<DatabasePlugin>().AsImplementedInterfaces().AsSelf();
            IContainer container = pluginBuilder.Build();

            var coreBuilder = new ContainerBuilder();
            coreBuilder.RegisterAssemblyTypes(typeof(Program).Assembly).AsSelf().AsImplementedInterfaces().SingleInstance();
            coreBuilder.RegisterAssemblyTypes(typeof(MasterCommunicator).Assembly).AsSelf().AsImplementedInterfaces().SingleInstance();
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

            //coreBuilder.Register(_ => new ToolkitMapper()).As<Mapper>().SingleInstance();
            return coreBuilder.Build();
        }
        public static void Main(string[] args)
        {
            try
            {
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

                bool ignoreStartupMessages = false;
                foreach (string arg in args)
                {
                    switch (arg)
                    {
                        case "--nomsg":
                            ignoreStartupMessages = true;
                            break;
                    }
                }

                int port = 4545;
                if (!ignoreStartupMessages)
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                    string text = $"MASTER SERVER v{fileVersionInfo.ProductVersion}dev - PORT : {port}";
                    int offset = (Console.WindowWidth / 2) + (text.Length / 2);
                    string separator = new string('=', Console.WindowWidth);
                    Console.WriteLine(separator + string.Format("{0," + offset + "}\n", text) + separator);
                }

                try
                {
                    using (IContainer coreContainer = InitializePlugins())
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


                        // initialize Logger
                        Logger.InitializeLogger(coreContainer.Resolve<ILogger>());

                        // initialize DB
                        if (!DataAccessHelper.Initialize(coreContainer.Resolve<IOpenNosContextFactory>()))
                        {
                            Console.ReadLine();
                            return;
                        }

                        Logger.Log.Info(Language.Instance.GetMessageFromKey("CONFIG_LOADED"));
                        // configure Services and Service Host
                        string ip = "127.0.0.1";
                        var serviceImpl = coreContainer.Resolve<MasterImpl>();
                        var server = new Server
                        {
                            Services = { global::Master.BindService(serviceImpl) },
                            Ports = { new ServerPort(ip, port, ServerCredentials.Insecure) }
                        };
                        Logger.Log.Info($"[RPC-SERVER] Listening on {ip}:{port}");
                        server.Start();
                        Console.Title = $"GloomyTale - Master | {ip}:{port}";

                        for (; ; )
                        {
                            string line = Console.ReadLine();
                            if (line == string.Empty || line == "quit")
                            {
                                break;
                            }
                        }

                        server.ShutdownAsync().ConfigureAwait(false).GetAwaiter().GetResult();


                    }
                }
                catch (Exception ex)
                {
                    Logger.Log.Error("General Error Server", ex);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
            }
        }

        #endregion
    }
}