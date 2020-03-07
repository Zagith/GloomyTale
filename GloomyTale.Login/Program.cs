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
using AutoMapper;
using GloomyTale.Communication;
using GloomyTale.Communication.RPC;
using GloomyTale.Core;
using GloomyTale.DAL;
using GloomyTale.DAL.EF.Helpers;
using GloomyTale.Data;
using GloomyTale.GameObject;
using GloomyTale.GameObject.Networking;
using GloomyTale.Handler;
using GloomyTale.NetworkManager;
using GloomyTale.Plugins;
using GloomyTale.Plugins.Logging;
using GloomyTale.Plugins.Logging.Interface;
using GloomyTale.Plugins.Modules;
using GloomyTale.SqlServer;
using GloomyTale.SqlServer.Mapping;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace GloomyTale.Login
{
    public static class Program
    {
        #region Members

        private static bool _isDebug;

        private static int _port;

        #endregion

        #region Methods

        private static void InitializeMasterCommunication()
        {
            int masterPort = Convert.ToInt32(Environment.GetEnvironmentVariable("MASTER_PORT") ?? "4545");
            string masterIp = Environment.GetEnvironmentVariable("MASTER_IP") ?? "127.0.0.1";
            var serviceFactory = new GRpcCommunicationServiceFactory();
            ICommunicationService service = serviceFactory.CreateService(masterIp, masterPort).ConfigureAwait(false).GetAwaiter().GetResult();
            CommunicationServiceClient.Initialize(service);
        }

        public static void Main(string[] args)
        {
            checked
            {
                try
                {
#if DEBUG
                    _isDebug = true;
#endif
                    CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
                    Console.Title = $"OpenNos Login Server{(_isDebug ? " Development Environment" : "")}";

                    bool ignoreStartupMessages = false;
                    foreach (string arg in args)
                    {
                        ignoreStartupMessages |= arg == "--nomsg";
                    }

                    int port = 4000;
                    
                    int portArgIndex = Array.FindIndex(args, s => s == "--port");
                    if (portArgIndex != -1
                        && args.Length >= portArgIndex + 1
                        && int.TryParse(args[portArgIndex + 1], out port))
                    {
                        Console.WriteLine("Port override: " + port);
                    }
                    Console.Title = $"GloomyTale - Login Server - {port}";
                    _port = port;
                    if (!ignoreStartupMessages)
                    {
                        Assembly assembly = Assembly.GetExecutingAssembly();
                        FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                        string text = $"LOGIN SERVER v{fileVersionInfo.ProductVersion}dev - PORT : {port}";
                        int offset = (Console.WindowWidth / 2) + (text.Length / 2);
                        string separator = new string('=', Console.WindowWidth);
                        Console.WriteLine(separator + string.Format("{0," + offset + "}\n", text) + separator);
                    }

                    var pluginBuilder = new ContainerBuilder();
                    pluginBuilder.RegisterType<SerilogLogger>().AsImplementedInterfaces().AsSelf();
                    pluginBuilder.RegisterType<LoggingPlugin>().AsImplementedInterfaces().AsSelf();
                    pluginBuilder.RegisterType<DatabasePlugin>().AsImplementedInterfaces().AsSelf();
                    IContainer container = pluginBuilder.Build();

                    var coreBuilder = new ContainerBuilder();
                    foreach (ICorePlugin plugin in container.Resolve<IEnumerable<ICorePlugin>>())
                    {
                        plugin.OnLoad(coreBuilder);
                    }

                    coreBuilder.Register(_ => new ToolkitMapper()).As<IMapper>().SingleInstance();

                    using (IContainer coreContainer = coreBuilder.Build())
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

                        DAOFactory.Initialize(coreContainer.Resolve<DAOFactory>());

                        Logger.Log.Info(Language.Instance.GetMessageFromKey("CONFIG_LOADED"));

                        try
                        {
                            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
                        }
                        catch (Exception ex)
                        {
                            Logger.Log.Error("General Error", ex);
                        }

                        try
                        {
                            // initialize PacketSerialization
                            PacketFactory.Initialize<WalkPacket>();

                            var server = new LoginServer("185.181.10.221", port, new BasicSpamProtector());
                            server.Start();

                            for (; ; )
                            {
                                string tmp = Console.ReadLine();
                                if (tmp == "quit")
                                {
                                    break;
                                }
                            }

                            server.Stop();
                        }
                        catch (Exception ex)
                        {
                            Logger.Log.LogEventError("INITIALIZATION_EXCEPTION", "General Error Server", ex);
                            Console.ReadLine();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log.LogEventError("INITIALIZATION_EXCEPTION", "General Error", ex);
                    Console.ReadKey();
                }
            }
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Error((Exception)e.ExceptionObject);
            try
            {

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            
            Logger.Log.Debug("Login Server crashed! Rebooting gracefully...");
            Process.Start("OpenNos.Login.exe", $"--nomsg --port {_port}");
            Environment.Exit(1);
        }

        #endregion
    }
}